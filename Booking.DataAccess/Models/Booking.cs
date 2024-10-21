namespace Booking.DataAccess.Models;
public record Booking(DateTimeOffset DateTime, decimal Duration, int NoOfPersons, Guid CompanyId, Contact Contact)
{
	private Booking(DateTimeOffset DateTime, decimal Duration, int NoOfPersons, Guid CompanyId) :
		   this(DateTime, Duration, NoOfPersons, CompanyId, null!)
	{

	}

	public Guid Id { get; init; }
	public Guid TableId { get; init; }
}