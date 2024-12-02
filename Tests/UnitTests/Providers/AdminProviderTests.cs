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
        var companyResponse = new
        {
            Id = Fixture.Create<Guid>(),
            Name = Fixture.Create<string>(),
            Address = Fixture.Create<string>()
        };

		MessageHandler.When($"http://bookingadmin/companies/{companyResponse.Id}").RespondWithJson(companyResponse);

        var result = await subject.VerifyCompany(companyResponse.Id);

        result.Should().BeTrue();
    }

	[Fact]
	public async Task VerifyCompany_InValidCompanyResponse_ReturnsFalse()
	{
		var requestId = Fixture.Create<Guid>();
		var companyResponse = new
		{
			Id = Fixture.Create<Guid>(),
			Name = Fixture.Create<string>(),
			Address = Fixture.Create<string>()
		};

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