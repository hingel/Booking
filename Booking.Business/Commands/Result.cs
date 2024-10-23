namespace Booking.Business.Commands;

public record Result<T>(bool Success, string? Message, T? Response);
