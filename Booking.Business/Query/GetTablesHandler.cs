using Booking.Business.Commands;
using Booking.DataAccess;
using Booking.DataAccess.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Query;

public record GetTablesQuery() : IRequest<Result<Table[]>>;
public class GetTablesHandler(ApplicationDbContext dbContext) : IRequestHandler<GetTablesQuery, Result<Table[]>>
{
	public async Task<Result<Table[]>> Handle(GetTablesQuery request, CancellationToken cancellationToken) => 
		new Result<Table[]>(true, "Tables for company: ", await dbContext.Tables.ToArrayAsync(cancellationToken));	
}
