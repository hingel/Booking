using Booking.Business.Commands.Handlers;
using Booking.Business.Query.Handlers;
using Booking.Contract.Requests;
using Booking.DataAccess.Models;
using MediatR;
using NodaTime.Extensions;

namespace Booking.Host.Extensions;
public static class WebbApplicationExtensions
{
	public static WebApplication MapTableEndpoints(this WebApplication app)
	{
		app.MapPost("tables", async (IMediator mediator, CreateTableRequest table, CancellationToken cancellation) =>
		{
			var result = await mediator.Send(new CreateTable(table.Name, Guid.Parse(table.CompanyId)), cancellation);
			return result.Success ? Results.Ok(result) : Results.Conflict(result);
		}).RequireAuthorization();

		app.MapGet("tables", async (IMediator mediator, IHttpContextAccessor httpContext, CancellationToken cancellation) =>
		{
			if (httpContext.HttpContext == null) throw new Exception("Context is null");

			var user = httpContext.HttpContext.User;
			var tid =  user.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new Exception("tid is null");

			return Results.Ok(await mediator.Send(new GetTablesQuery(Guid.Parse(tid.Value)), cancellation));
		}).RequireAuthorization();

		return app;
	}

	public static WebApplication MapBookingEndpoints(this WebApplication app)
	{
		app.MapPost("bookings", async (IMediator mediator, CreateBookingRequest booking, CancellationToken cancellation) =>
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

		app.MapGet("bookings/{id}", async (IMediator mediator, string id, CancellationToken cancellation) =>
		{
			var result = await mediator.Send(new GetBookingById(Guid.Parse(id)), cancellation);
			return result.Success ? Results.Ok(result) : Results.NotFound(result);
		}); //Denna skulle kunna vara bunden mot epost och mot id samtidigt.

		app.MapGet("bookings/query/{companyId}/{fromdate}", async (IMediator mediator, string companyId, DateOnly fromdate, CancellationToken cancellation) =>
			Results.Ok(await mediator.Send(new BookingQuery(Guid.Parse(companyId), fromdate.ToLocalDate()), cancellation)));

		return app;
	}
}