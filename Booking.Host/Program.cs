using Booking.Business.Commands.Handlers;
using Booking.Business.Repository;
using Booking.DataAccess.Models;
using Booking.Host.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LocalConnectionString") ?? throw new Exception("Connectionstring not found");

builder.Services.AddDbContext<Booking.DataAccess.ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IRepository, TableRepository>();
builder.Services.AddMediatR(cfg => 	cfg.RegisterServicesFromAssemblies(typeof(TableRepository).Assembly));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/", async (IMediator mediator, CreateBookingRequest booking) =>
	Results.Ok(await mediator.Send(new CreateBooking(Guid.NewGuid(),
	booking.DateTime,
	booking.Duration,
	booking.Persons,
	new Contact(booking.Contact.Name,
	booking.Contact.PhoneNumber,
	booking.Contact.Email),
	Guid.Parse(booking.CompanyId)))));
	//.AddEndpointFilter(async (invocationContext, next) =>
	//{
	//	invocationContext.HttpContext.Request.Headers.TryGetValue("CompanyId", out var companyIdString);

	//	if (string.IsNullOrEmpty(companyIdString)) return Results.Problem("No company id");

	//	var companyId = Guid.Parse(companyIdString!);
	//	//TODO, kan lägga till detta i en klass för att använda som queryparameter

	//	return await next(invocationContext);
	//});

app.MapPost("tables", async (IMediator mediator, CreateTableRequest table) =>
{
	var result = await mediator.Send(new CreateTable(table.Name, Guid.Parse(table.CompanyId)));
	return result.Success ? Results.Ok(result) : Results.Conflict(result); 
});

app.Run();
