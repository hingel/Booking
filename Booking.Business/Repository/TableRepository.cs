
using Booking.DataAccess;
using Booking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Repository;

public class TableRepository(ApplicationDbContext dbContext) : IRepository
{
	public async Task<Table[]> Tables(Guid companyId) => await dbContext.Tables.Where(t => t.CompanyId == companyId).ToArrayAsync();

	public async Task SaveChanges() => await dbContext.SaveChangesAsync();
}

