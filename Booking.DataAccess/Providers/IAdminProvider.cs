namespace Booking.DataAccess.Providers;
public interface IAdminProvider
{
	Task<bool> VerifyCompany(Guid id);
}