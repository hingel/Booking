using Booking.Business.Commands.Handlers;
using Booking.DataAccess;
using Booking.DataAccess.Providers;
using Booking.Host.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(); //Lägg till policy.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
	options.Audience = "BookingAppAudience";
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidIssuer = "BookingApp",
		ValidAudience = "BookingAppAudience",
		ValidateIssuerSigningKey = true, //TODO: Kolla om denna behövs
		IssuerSigningKeys = [new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ValidationToken:Value"] ??
		throw new SecurityTokenException("Key value not found")))],
	};
}); //JwtBearerDefaults.AuthenticationScheme = "Bearer" egentligen


builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("LocalConnectionString") ?? throw new Exception("Connectionstring not found");
//var host = Environment.GetEnvironmentVariable("DB_HOST");
//var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
//var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
//var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

var stringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
{
	//Host = host,
	//Database = database,
	//Username = username,
	//Password = password
	Password = builder.Configuration["PostgreSQL:Password"]
};

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
	stringBuilder.ConnectionString, 
	o => o.UseNodaTime()));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateBookingHandler).Assembly));

builder.Services.AddHttpClient<IAdminProvider, AdminProvider>((client) =>
{
	client.BaseAddress = new Uri(builder.Configuration.GetSection("Services").GetValue<string>("Admin")
	?? throw new Exception("Base address not found"));
}).ConfigureHttpClient((services, client) =>
{
	var accessor = services.GetRequiredService<IHttpContextAccessor>();
	if (accessor == null || accessor.HttpContext == null) throw new Exception("Accesor or context is null");
	
	var token = accessor.HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "Authorization");
	if (!string.IsNullOrEmpty(token.Value)) 
		client.DefaultRequestHeaders.Add(token.Key, token.Value.ToString());	
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) //Detta görs på något annat sätt:
{
	using var scope = app.Services.CreateScope();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
	logger.LogInformation("In Development");
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => $"Hello World, försök att gå tag på environment variabel: Postgresdb är: {Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "Could not be found"}");

//Denna token generering ska enbart ligga i Gatewayen egentligen:
app.MapPost("/createToken/{userId}/{companyId}", (string userId, string companyId) =>
{
	var subClaim = new Claim("sub", userId);
	var companyClaim = new Claim("tid", companyId);
	var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ValidationToken:Value"] ??
		throw new SecurityTokenException("Key value not found")));
	var tokenOptions = new JwtSecurityToken(
		issuer: "BookingApp",
		audience: "BookingAppAudience",
		claims: [subClaim, companyClaim],
		expires: DateTime.Now.AddDays(2),
		signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
	);

	return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
});

app.MapTableEndpoints();
app.MapBookingEndpoints();

app.Run();

public partial class Program { }
