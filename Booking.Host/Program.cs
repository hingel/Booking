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
	Guid.Parse(booking.TableId),
	Guid.Parse(booking.CompanyId)))));

app.MapPost("tables/", () => "Table");

app.Run();
