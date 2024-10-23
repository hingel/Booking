namespace Booking.DataAccess.Models;
public record Table(string Name, Guid CompanyId)
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public ICollection<Booking> Bookings { get; } = new List<Booking>();
}
