using AutoFixture;
using Booking.Business.Commands.Handlers;
using FluentAssertions;

namespace UnitTests.Business;
public class CreateTableHandlerTests : UnitTests
{
    private readonly CreateTableHandler subject;
    public CreateTableHandlerTests()
    {
        subject = new CreateTableHandler(DbContext);
    }

    [Fact]
    public async Task Handle_WithTakenName_ReturnsFalse()
    {
        var request = Fixture.Create<CreateTable>();
        var table = Fixture.Build<Booking.DataAccess.Models.Table>()
            .With(t => t.CompanyId, request.CompanyId)
            .With(t => t.Name, request.Name).Create();

        DbContext.Tables.Add(table);
        await DbContext.SaveChangesAsync();

        var response = await subject.Handle(request, CancellationToken.None);
        response.Should().BeEquivalentTo(new
        {
            Success = false,
            Message = "Table already exists",
            Data = Guid.Empty
        });
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