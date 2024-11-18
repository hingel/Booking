using Booking.Contract.Responses;

namespace Booking.Contract.Requests;
public record TableResponse(Guid Id, string Name, Guid CompanyId, BookingResponse[]? BookingResponses);
