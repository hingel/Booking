using AutoFixture;
using UnitTests.SpecimenBuilder;

namespace UnitTests;
public class UnitTests
{
	public Fixture Fixture { get; init; }

	public UnitTests()
	{
		Fixture = new Fixture();
		Fixture.Customizations.Add(new DateSpecimenBuilder());
	}
}

