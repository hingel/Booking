namespace Booking.Contract.Requests
{
    public record CreateBookingRequest(DateTimeOffset DateTime, int Duration, int Persons, ContactContract Contact, string TableId, string CompanyId);

    public record ContactContract(string Name, string PhoneNumber, string Email);
}
