using Booking.DataAccess;
using Booking.DataAccess.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Booking.Business.Commands.Handlers;

public record CreateBooking(Guid Id, LocalDateTime DateTime, int Duration, int Persons, Contact Contact, Guid CompanyId) : IRequest<Result<string>>;

public class CreateBookingHandler(ApplicationDbContext dbContext, ILogger<CreateBookingHandler> logging) : IRequestHandler<CreateBooking, Result<string>>
{
	public async Task<Result<string>> Handle(CreateBooking request, CancellationToken cancellationToken)
	{
		logging.LogInformation("Booking.Handlers.CreateBooking {booking}", request.Id);

		var availableTables = await dbContext.Tables.Where(t => t.CompanyId == request.CompanyId && !t.Bookings.Any(b =>
		request.DateTime.Year == b.DateTime.Year &&
		request.DateTime.DayOfYear == b.DateTime.DayOfYear &&
		request.DateTime.Hour + request.Duration > b.DateTime.Hour &&
		request.DateTime.Hour < b.DateTime.Hour + b.Duration)).ToArrayAsync(cancellationToken);

		if (availableTables.Length == 0) return new Result<string>(false, "Booking not added", null);

		var bookingToAdd = new DataAccess.Models.Booking(
			request.DateTime,
			request.Duration,
			request.Persons,
			request.CompanyId
			)
		{
			Contact = request.Contact
		};

		availableTables.First().Bookings.Add(bookingToAdd);

		await dbContext.SaveChangesAsync(cancellationToken);
		return new Result<string>(true, "Booking added", bookingToAdd.Id.ToString());
	}
}