using AutoFixture;
using Booking.Business.Commands.Handlers;
using Booking.DataAccess.Providers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests.Business;
public class CreateTableHandlerTests : UnitTests
{
	private readonly IAdminProvider adminProvider;
	private readonly CreateTableHandler subject;
    public CreateTableHandlerTests()
    {
		var logger = Substitute.For<ILogger<CreateTableHandler>>();
        adminProvider = Substitute.For<IAdminProvider>();
		subject = new CreateTableHandler(DbContext, adminProvider, logger);
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

        adminProvider.VerifyCompany(request.CompanyId).Returns(true);

        var response = await subject.Handle(request, CancellationToken.None);
        response.Should().BeEquivalentTo(new
        {
            Success = false,
            Message = "Table already exists",
            Data = Guid.Empty
        });
    }

	[Fact]
	public async Task Handle_CompanyDoesNotExists_ReturnsFalse()
	{
		var request = Fixture.Create<CreateTable>();

		adminProvider.VerifyCompany(request.CompanyId).Returns(false);

		var response = await subject.Handle(request, CancellationToken.None);
		response.Should().BeEquivalentTo(new
		{
			Success = false,
			Message = "Company does not exist",
			Data = Guid.Empty
		});
	}

	[Fact]
    public async Task Handle_ValidRequest_TableSaved()
    {
        var request = Fixture.Create<CreateTable>();

        adminProvider.VerifyCompany(request.CompanyId).Returns(true);

        var response = await subject.Handle(request, CancellationToken.None);
        response.Should().BeEquivalentTo(new
        {
            Success = true,
            Message = $"Table with name {request.Name} added"
        });
    }
}