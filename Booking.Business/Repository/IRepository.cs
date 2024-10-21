using Booking.Business.Commands.Handlers;
using Booking.DataAccess.Models;

namespace Booking.Business.Repository;
public interface IRepository
{
	Task<IEnumerable<Table>> GetAllTables(Guid CompanyId);
	Task SaveChanges();
	Task<Guid> AddTable(Table table);
	Task<Table[]> GetAvailableTables(CreateBooking request);
}