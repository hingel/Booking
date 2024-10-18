using Booking.Business.Commands.Handlers;
using Booking.DataAccess;
using Booking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Repository;

public class TableRepository(ApplicationDbContext dbContext) : IRepository
{
	public async Task<IEnumerable<Table>> GetAllTables(Guid companyId) => await dbContext.Tables.Where(t => t.CompanyId == companyId).ToListAsync();

	public async Task SaveChanges() => await dbContext.SaveChangesAsync();

	public async Task AddTable(Table table)
	{
		dbContext.Tables.Add(table);
		await dbContext.SaveChangesAsync();
	}

	public async Task<Table[]> GetAvailableTables(CreateBooking request)
	{
		var tables = await dbContext.Tables.Where(t => t.CompanyId == request.CompanyId && !t.Bookings.Any(b =>
		request.DateTime.Year == b.DateTime.Year &&
		request.DateTime.DayOfYear == b.DateTime.DayOfYear &&
		request.DateTime.Hour + request.Duration > b.DateTime.Hour &&
		request.DateTime.Hour < b.DateTime.Hour + b.Duration)).ToArrayAsync();

		return tables;
	}
}

