using Booking.Business.Commands.Handlers;
using Booking.Business.Repository;
using Booking.DataAccess.Models;
using Booking.Host.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LocalConnectionString") ?? throw new Exception("Connectionstring not found");

var stringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
{
	Password = builder.Configuration["PostgreSQL:Password"]
};

builder.Services.AddDbContext<Booking.DataAccess.ApplicationDbContext>(options => options.UseNpgsql(stringBuilder.ConnectionString, o => o.UseNodaTime()));
builder.Services.AddScoped<IRepository, TableRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(TableRepository).Assembly));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

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

app.Run();
