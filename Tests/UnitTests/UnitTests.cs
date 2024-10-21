using AutoFixture;
using Booking.Business.Commands.Handlers;
using Booking.Business.Repository;
using UnitTests.SpecimenBuilder;

namespace UnitTests;
public class UnitTests
{
	public Fixture Fixture { get; init; }
	private readonly IRepository repository;
	private readonly CreateBookingHandler subject;

	public UnitTests()
	{
		Fixture = new Fixture();
		Fixture.Customizations.Add(new DateSpecimenBuilder());
	}
}

