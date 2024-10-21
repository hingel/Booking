using NodaTime;

namespace Booking.DataAccess.Models;
public record Booking(LocalDateTime DateTime, decimal Duration, int NoOfPersons, Guid CompanyId)
{
	//public Booking(DateTime DateTime, decimal Duration, int NoOfPersons, Guid CompanyId) :
	//	   this(DateTime, Duration, NoOfPersons, CompanyId, null!)
	//{

	//}

	public Contact Contact { get; init; } = null!;

	public Guid Id { get; init; }
	public Guid TableId { get; init; }
}