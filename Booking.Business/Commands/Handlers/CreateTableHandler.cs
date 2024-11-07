using Booking.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Commands.Handlers;

public record CreateTable(string Name, Guid CompanyId) : IRequest<Result<Guid>>;

public class CreateTableHandler(ApplicationDbContext context) : IRequestHandler<CreateTable, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateTable request, CancellationToken cancellationToken)
	{
		if ((await context.Tables.Where(t => t.CompanyId == request.CompanyId).AnyAsync(t => t.Name == request.Name)))
		{
			return new Result<Guid>(false, "Table already exists", Guid.Empty);
		}

		var tableToAdd = new DataAccess.Models.Table(request.Name, request.CompanyId);
		context.Tables.Add(tableToAdd);
		await context.SaveChangesAsync();

		return new Result<Guid>(true, $"Table with name {request.Name} added", tableToAdd.Id); 
	}
}
