using Booking.DataAccess.Models;

namespace Booking.Business.Repository;
public interface IRepository
{
	Task<Table[]> Tables(Guid CompanyId);
	Task SaveChanges();
}