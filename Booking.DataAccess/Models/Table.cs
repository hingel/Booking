namespace Booking.DataAccess.Models;
public record Table(string Name, Guid CompanyId)
{
	public Guid Id { get; init; }
	public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
