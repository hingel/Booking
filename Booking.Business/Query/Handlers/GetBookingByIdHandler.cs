using Booking.Business.Mapping;
using Booking.Contract.Responses;
using Booking.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Query.Handlers;

public record GetBookingById(Guid Id) : IRequest<Result<BookingResponse>>;

public class GetBookingByIdHandler(ApplicationDbContext dbContext) : IRequestHandler<GetBookingById, Result<BookingResponse>>
{
	public async Task<Result<BookingResponse>> Handle(GetBookingById request, CancellationToken cancellationToken)
	{
		var table =	await dbContext.Tables.FirstOrDefaultAsync(t => t.Bookings.Any(b => b.Id == request.Id));

		return table == null ? new Result<BookingResponse>(false, $"Booking with id: {request.Id} not found", null) : 
			new Result<BookingResponse>(true, $"Booking with id: {request.Id} found", table.Bookings.First(b => b.Id == request.Id).ToContract());
	}
}