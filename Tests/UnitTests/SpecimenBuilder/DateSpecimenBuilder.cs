using AutoFixture.Kernel;
using NodaTime;

namespace UnitTests.SpecimenBuilder;
public class DateSpecimenBuilder : ISpecimenBuilder
{
	public object Create(object request, ISpecimenContext context)
	{
		var type = request as Type;

		if (type == null) return new NoSpecimen();

		if (type != typeof(LocalDateTime)) return new NoSpecimen();

		var rnd = new Random();
		return new LocalDateTime(rnd.Next(2000, 2025), rnd.Next(1,13), rnd.Next(1, 28), rnd.Next(1,24), rnd.Next(1, 60), rnd.Next(1, 60));
	}
}
