using Booking.Business.Mapping;
using Booking.Contract.Requests;
using Booking.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Query.Handlers;

public record GetTablesQuery(Guid CompanyId) : IRequest<Result<TableResponse[]>>;
public class GetTablesHandler(ApplicationDbContext dbContext) : IRequestHandler<GetTablesQuery, Result<TableResponse[]>>
{
    public async Task<Result<TableResponse[]>> Handle(GetTablesQuery request, CancellationToken cancellationToken) => 
        new Result<TableResponse[]>(true,
            "Tables for company: ",
            (await dbContext.Tables.Where(t => t.CompanyId == request.CompanyId).ToArrayAsync(cancellationToken))
            .Select(t => t.ToContract()).ToArray());
}
