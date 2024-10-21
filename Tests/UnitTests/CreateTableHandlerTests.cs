using AutoFixture;
using Booking.Business.Commands.Handlers;
using Booking.Business.Repository;
using FluentAssertions;
using NSubstitute;

namespace UnitTests;
public class CreateTableHandlerTests : UnitTests
{
    private readonly IRepository repository;
    private readonly CreateTableHandler subject;
    public CreateTableHandlerTests()
    {
		repository = Substitute.For<IRepository>();
		subject = new CreateTableHandler(repository);
    }

    [Fact]
    public async Task Handle_WithTakenName_ReturnsFalse()
    {
        var request = Fixture.Create<CreateTable>();
        var table = Fixture.Build<Booking.DataAccess.Models.Table>()
            .With(t => t.CompanyId, request.CompanyId)
            .With(t => t.Name, request.Name).Create();

        repository.GetAllTables(request.CompanyId).Returns([table]);

        var response = await subject.Handle(request, CancellationToken.None);
        response.Should().BeEquivalentTo(new { 
        Success = false,
        Message = "Table already exists",
        Response = Guid.Empty});
    }

    [Fact]
    public async Task Handle_ValidRequest_TableSaved()
    {
		var request = Fixture.Create<CreateTable>();

		var response = await subject.Handle(request, CancellationToken.None);
		response.Should().BeEquivalentTo(new
		{
			Success = true,
			Message = $"Table with name {request.Name} added"
		});
	}
}