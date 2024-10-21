using Booking.Business.Repository;
using Booking.DataAccess.Models;
using MediatR;
using NodaTime;

namespace Booking.Business.Commands.Handlers;

public record CreateBooking(Guid Id, LocalDateTime DateTime, int Duration, int Persons, Contact Contact, Guid CompanyId) : IRequest<Result<string>>;

public class CreateBookingHandler(IRepository tableRepository) : IRequestHandler<CreateBooking, Result<string>>
{
	public async Task<Result<string>> Handle(CreateBooking request, CancellationToken cancellationToken)
	{
		var availableTables = await tableRepository.GetAvailableTables(request);

		if (!availableTables.Any()) return new Result<string>(false, "Booking not added", null);

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

		await tableRepository.SaveChanges();
		return new Result<string>(true, "Booking added", bookingToAdd.Id.ToString());
	}
}

