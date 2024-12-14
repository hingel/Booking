using AutoFixture;
using Booking.Business.Mapping;
using Booking.Business.Query.Handlers;
using Booking.DataAccess.Models;
using FluentAssertions;
using NodaTime;

namespace UnitTests.Business;

public class BookingQueryHandlerTests : UnitTests
{
	private readonly BookingQueryHandler subject;

	public BookingQueryHandlerTests()
	{
		subject = new BookingQueryHandler(DbContext);
	}

	[Fact]
	public async Task GetBookings_Returns()
	{
		var companyId = Fixture.Create<Guid>();
		var date = Fixture.Create<LocalDateTime>();

		var table = Fixture.Create<Table>() with { CompanyId = companyId };

		AddTestBookings(companyId, date, ref table);

		DbContext.Tables.Add(table);
		await DbContext.SaveChangesAsync();

		var request = new BookingQuery(companyId, new LocalDate(date.Year, date.Month, date.Day + 1));

		var result = await subject.Handle(request, CancellationToken.None);

		result.Should().BeEquivalentTo(new {
			Success = true,
			Message = $"Booking for company: {request.CompanyId} from {request.Fromdate}",
			Data = table.Bookings.Skip(1).Select(b => b.ToContract()).ToArray()
		});
	}

	[Fact]
	public async Task GetBookings_ReturnsNoBookings()
	{
		var companyId = Fixture.Create<Guid>();
		var date = Fixture.Create<LocalDateTime>();

		var table = Fixture.Create<Table>() with { CompanyId = companyId };

		AddTestBookings(companyId, date, ref table);

		DbContext.Tables.Add(table);
		await DbContext.SaveChangesAsync();

		var request = new BookingQuery(companyId, new LocalDate(date.Year, date.Month, date.Day + 10));

		var result = await subject.Handle(request, CancellationToken.None);

		result.Should().BeEquivalentTo(new
		{
			Success = false,
			Message = "no bookings found",
			Data = null as Table[]		
		});
	}

	private void AddTestBookings(Guid companyId, LocalDateTime date, ref Table table)
	{
		for (int i = 0; i < 3; i++)
		{
			table.Bookings.Add(Fixture.Create<Booking.DataAccess.Models.Booking>() with 
			{ 
				CompanyId = companyId,
				DateTime = date.PlusDays(i)
			});
		}
	}
}
