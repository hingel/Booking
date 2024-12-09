using AutoFixture;
using Booking.DataAccess;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using UnitTests.SpecimenBuilder;

namespace UnitTests;
public class UnitTests : IDisposable
{
	public Fixture Fixture { get; init; }
	public ApplicationDbContext DbContext { get; }
	public MockHttpMessageHandler MessageHandler { get; init; }
	public HttpClient Client { get; set; }

	public UnitTests()
	{
		Fixture = new Fixture();
		Fixture.Customizations.Add(new DateSpecimenBuilder());
		var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("TestDataBase").Options;
		DbContext = new ApplicationDbContext(options);

		MessageHandler = new MockHttpMessageHandler();
		Client =  MessageHandler.ToHttpClient();
		Client.BaseAddress = new Uri("http://bookingadmin");
		//Client.DefaultRequestHeaders.Add("Authorization", "token");
	}

	public void Dispose()
	{
		DbContext.Dispose();
	}
}

