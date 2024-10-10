using Booking.Business.Repository;
using Booking.DataAccess.Models;
using MediatR;

namespace Booking.Business.Commands.Handlers;

public record CreateBooking(Guid Id, DateTimeOffset DateTime, int Duration, int Persons, Contact Contact, Guid TableId, Guid CompanyId) : IRequest<string>;

public class CreateBookingHandler(IRepository tableRepository) : IRequestHandler<CreateBooking, string>
{
	public async Task<string> Handle(CreateBooking request, CancellationToken cancellationToken)
	{
		var availableTables = (await tableRepository.Tables(request.CompanyId))
		.Where(t => !t.Bookings.Any(b =>
		request.DateTime.Year == b.DateTime.Year &&
		request.DateTime.DayOfYear == b.DateTime.DayOfYear &&
		request.DateTime.Hour + request.Duration > b.DateTime.Hour &&
		request.DateTime.Hour < b.DateTime.Hour + b.Duration));

		if (!availableTables.Any()) return "Booking not added";

		availableTables.First().Bookings.Add(
			new DataAccess.Models.Booking(request.Id,
			request.DateTime,
			request.Duration,
			request.Persons,
			request.CompanyId,
			request.Contact));

		await tableRepository.SaveChanges();
		return "Booking added";
	}
}

