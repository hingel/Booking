using AutoFixture;
using Booking.DataAccess;
using Microsoft.EntityFrameworkCore;
using UnitTests.SpecimenBuilder;

namespace UnitTests;
public class UnitTests : IDisposable
{
	public Fixture Fixture { get; init; }
	public ApplicationDbContext DbContext { get; }

	public UnitTests()
	{
		Fixture = new Fixture();
		Fixture.Customizations.Add(new DateSpecimenBuilder());
		var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("TestDataBase").Options;
		DbContext = new ApplicationDbContext(options);
	}

	public void Dispose()
	{
		DbContext.Dispose();
	}
}

