using AutoFixture;
using Booking.Business.Mapping;
using Booking.Business.Query.Handlers;
using Booking.DataAccess.Models;
using FluentAssertions;

namespace UnitTests.Business;

public class GetTablesHandlerTests : UnitTests
{
    private readonly GetTablesHandler subject;

    public GetTablesHandlerTests()
    {
        subject = new GetTablesHandler(DbContext);
    }

    [Fact]
    public async Task Handle_QueryTables_ReturnsTables()
    {
        var tables = Fixture.CreateMany<Table>();
        DbContext.Tables.AddRange(tables);
        await DbContext.SaveChangesAsync();

        var result = await subject.Handle(Fixture.Build<GetTablesQuery>().With(t => t.CompanyId, tables.ElementAt(0).CompanyId).Create(), CancellationToken.None);
        result.Should().BeEquivalentTo(new
        {
            Success = true,
            Message = "Tables for company: ",
        });

        result.Data.Should().Contain(tables.ElementAt(0).ToContract());
    }
}
