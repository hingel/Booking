namespace Booking.DataAccess.Models;
public record Booking(Guid Id, DateTimeOffset DateTime, decimal Duration, int NoOfPersons, Guid CompanyId, Contact Contact)
{
	private Booking(Guid Id, DateTimeOffset DateTime, decimal Duration, int NoOfPersons, Guid CompanyId) :
		   this(Id, DateTime, Duration, NoOfPersons, CompanyId, null!)
	{

	}

	//public Contact Contact { get; } = null!;
	public Table Table { get; } = null!;
}