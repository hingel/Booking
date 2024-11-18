using Booking.Business.Commands.Handlers;
using Booking.DataAccess;
using Booking.Host.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

//var connectionString = builder.Configuration.GetConnectionString("LocalConnectionString") ?? throw new Exception("Connectionstring not found");

var host = Environment.GetEnvironmentVariable("DB_HOST");
var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

var stringBuilder = new NpgsqlConnectionStringBuilder() //connectionString)
{
	Host = host,
	Database = database,
	Username = username,
	Password = password
	//Password = builder.Configuration["PostgreSQL:Password"]
};

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
	stringBuilder.ConnectionString, 
	o => o.UseNodaTime()));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateBookingHandler).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) //Detta görs på något annat sätt:
{
	using var scope = app.Services.CreateScope();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
	logger.LogInformation("In Development");
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();
}

app.MapGet("/", () => $"Hello World, försök att gå tag på environment variabel: Postgresdb är: {Environment.GetEnvironmentVariable("POSTGRES_DB")}");

app.MapTableEndpoints();
app.MapBookingEndpoints();

app.Run();

public partial class Program { }
