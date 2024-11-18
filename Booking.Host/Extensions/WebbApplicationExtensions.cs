using Booking.Business.Commands.Handlers;
using Booking.Business.Query.Handlers;
using Booking.Contract.Requests;
using Booking.DataAccess.Models;
using MediatR;

namespace Booking.Host.Extensions;
public static class WebbApplicationExtensions
{
	public static WebApplication MapTableEndpoints(this WebApplication app)
	{ 
		app.MapPost("tables", async (IMediator mediator, CreateTableRequest table, CancellationToken cancellation) =>
		{
			var result = await mediator.Send(new CreateTable(table.Name, Guid.Parse(table.CompanyId)), cancellation);
			return result.Success ? Results.Ok(result) : Results.Conflict(result);
		});

		app.MapGet("tables", async (IMediator mediator, CancellationToken cancellation) =>
			Results.Ok(await mediator.Send(new GetTablesQuery(), cancellation)));

		return app;
	}

	public static WebApplication MapBookingEndpoints(this WebApplication app)
	{
		app.MapPost("/", async (IMediator mediator, CreateBookingRequest booking, CancellationToken cancellation) =>
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
				Guid.Parse(booking.CompanyId)), cancellation);

			return result.Success ? Results.Ok(result) : Results.Conflict(result);
		});

		app.MapGet("/bookings/{id}", async (IMediator mediator, string id, CancellationToken cancellation) =>
		{
			var result = await mediator.Send(new GetBookingById(Guid.Parse(id)), cancellation);
			return result.Success ? Results.Ok(result) : Results.NotFound(result);
		});

		return app;
	}
}