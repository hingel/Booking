using AutoFixture;
using Booking.DataAccess.Providers;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace UnitTests.Providers;

public class AdminProviderTests : UnitTests
{
	private readonly AdminProvider subject;

	public AdminProviderTests()
    {
        subject = new AdminProvider(Client);
    }

    [Fact]
    public async Task VerifyCompany_ValidCompanyResponse_ReturnsTrue()
    {
		var companyId = Guid.NewGuid();
        var companyResponse = new { Success = true };

		MessageHandler.When($"http://bookingadmin/companies/{companyId.ToString()}").RespondWithJson(companyResponse);

        var result = await subject.VerifyCompany(companyId);

        result.Should().BeTrue();
    }

	[Fact]
	public async Task VerifyCompany_InValidCompanyResponse_ReturnsFalse()
	{
		var requestId = Fixture.Create<Guid>();
		var companyResponse = new { Success = false };

		MessageHandler.When($"http://bookingadmin/companies/{requestId}").RespondWithJson(companyResponse);

		var result = await subject.VerifyCompany(requestId);

		result.Should().BeFalse();
	}

	[Fact]
	public async Task VerifyCompany_NullCompanyResponse_ReturnsFalse()
	{
		var requestId = Fixture.Create<Guid>();
		MessageHandler.When($"http://bookingadmin/companies/{requestId}").RespondWithJson(null);

		var result = await subject.VerifyCompany(requestId);

		result.Should().BeFalse();
	}
}