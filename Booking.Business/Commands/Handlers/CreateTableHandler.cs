using Booking.Business.Repository;
using MediatR;

namespace Booking.Business.Commands.Handlers;

public record CreateTable(string Name, Guid CompanyId) : IRequest<Result<Guid>>;

public class CreateTableHandler(IRepository tableRepository) : IRequestHandler<CreateTable, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateTable request, CancellationToken cancellationToken)
	{
		if ((await tableRepository.GetAllTables(request.CompanyId)).Any(t => t.Name == request.Name))
		{
			return new Result<Guid>(false, "Table already exists", Guid.Empty);
		}

		var addedTableId = await tableRepository.AddTable(new DataAccess.Models.Table(request.Name, request.CompanyId));

		return new Result<Guid>(true, $"Table with name {request.Name} added", addedTableId); 
	}
}
