using Booking.Business.Commands.Handlers;
using Booking.DataAccess;
using Booking.DataAccess.Models;
using Booking.Host.Contracts;
using MediatR;
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

if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
	logger.LogInformation("In Development");
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();
}
else
{
	using var scope = app.Services.CreateScope();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
	logger.LogInformation("Is not in Development");
}

app.MapGet("/", () => $"Hello World, försök att gå tag på environment variabel: Postgresdb är: {Environment.GetEnvironmentVariable("POSTGRES_DB")}");

app.MapPost("/", async (IMediator mediator, CreateBookingRequest booking) =>
{
	var result = await mediator.Send(new CreateBooking(Guid.NewGuid(),
		new NodaTime.LocalDateTime(booking.DateTime.Year,
			booking.DateTime.Month,
			booking.DateTime.Day,
			booking.DateTime.Hour,
			booking.DateTime.Minute,
			booking.DateTime.Second),
		booking.Duration,
		booking.Persons,
		new Contact(booking.Contact.Name,
		booking.Contact.PhoneNumber,
		booking.Contact.Email),
		Guid.Parse(booking.CompanyId)));

	return result.Success ? Results.Ok(result) : Results.Conflict(result);
});

app.MapPost("tables", async (IMediator mediator, CreateTableRequest table) =>
{
	var result = await mediator.Send(new CreateTable(table.Name, Guid.Parse(table.CompanyId)));
	return result.Success ? Results.Ok(result) : Results.Conflict(result);
});

app.MapGet("tables", async (ApplicationDbContext context) => 
Results.Ok(await context.Tables.ToListAsync()));

app.Run();
