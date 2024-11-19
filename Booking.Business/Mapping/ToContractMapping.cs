using Booking.Contract.Requests;
using Booking.Contract.Responses;

namespace Booking.Business.Mapping;
public static class ToContractMapping
{
	public static BookingResponse ToContract(this DataAccess.Models.Booking b) =>
		new(b.Id,
			b.TableId,
			new DateTime(b.DateTime.Year, b.DateTime.Month, b.DateTime.Day, b.DateTime.Hour, b.DateTime.Minute, b.DateTime.Second),
			b.Duration,
			b.NoOfPersons,
			b.CompanyId,
			new ContactResponse(b.Contact.Name, b.Contact.PhoneNumber, b.Contact.Email));

	public static TableResponse ToContract(this DataAccess.Models.Table t) =>
		new (t.Id,
			t.Name,
			t.CompanyId,
			t.Bookings.Select(b => b.ToContract()).ToArray());
}