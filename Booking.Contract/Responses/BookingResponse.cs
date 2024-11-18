namespace Booking.Contract.Responses;
public record BookingResponse(Guid Id, 
	Guid TableId,
	DateTime DateTime,
	decimal Duration,
	int NoOfPersons, 
	Guid CompanyId,
	ContactResponse Contact);