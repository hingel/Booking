namespace Booking.DataAccess.Models;
public record Table(Guid Id, string Name, Guid CompanyId)
{
	public ICollection<Booking> Bookings { get; } = [];
}
