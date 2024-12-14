using Booking.Business.Mapping;
using Booking.Contract.Responses;
using Booking.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Booking.Business.Query.Handlers;

public record BookingQuery(Guid CompanyId, LocalDate Fromdate) : IRequest<Result<BookingResponse[]>>;

public class BookingQueryHandler(ApplicationDbContext dbContext) : IRequestHandler<BookingQuery, Result<BookingResponse[]>>
{
	public async Task<Result<BookingResponse[]>> Handle(BookingQuery request, CancellationToken cancellationToken)
	{
		var bookings = await dbContext.Tables
			.AsNoTracking()
			.Where(t => t.CompanyId == request.CompanyId)
			.Select(t => t.Bookings.Where(b => b.DateTime.Date >= request.Fromdate))
			.SelectMany(b => b)
			.ToArrayAsync(cancellationToken);

		return bookings.Length > 0 ? new Result<BookingResponse[]>(
			true,
			$"Booking for company: {request.CompanyId} from {request.Fromdate}", 
			bookings.Select(b => b.ToContract()).ToArray()) :
			new Result<BookingResponse[]>(false, "no bookings found", null);
	}
}