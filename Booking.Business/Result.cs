namespace Booking.Business;

public record Result<T>(bool Success, string? Message, T? Data);
