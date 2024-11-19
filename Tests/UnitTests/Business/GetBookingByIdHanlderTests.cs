using AutoFixture;
using Booking.Business.Query.Handlers;
using Booking.DataAccess.Models;
using FluentAssertions;
using System.Runtime.CompilerServices;

namespace UnitTests.Business;
public class GetBookingByIdHanlderTests : UnitTests
{
	private readonly GetBookingByIdHandler subject;

    public GetBookingByIdHanlderTests()
    {
        subject = new GetBookingByIdHandler(DbContext);
    }

    [Fact]
    public async Task Handle_NoBookingFound_ReturnsFalse()
    {
        var request = Fixture.Create<GetBookingById>();

		var response = await subject.Handle(request, CancellationToken.None);

        response.Should().BeEquivalentTo(new 
        { 
            Success = false,
            Message = $"Booking with id: {request.Id} not found"
		});
    }

	[Fact]
	public async Task Handle_BookingFound_Returns()
	{
		var request = Fixture.Create<GetBookingById>();
		var table = Fixture.Create<Table>();
		table.Bookings.Add(Fixture.Build<Booking.DataAccess.Models.Booking>().With(b => b.TableId, table.Id).Create());
		DbContext.Tables.Add(table);
		await DbContext.SaveChangesAsync();

		var response = await subject.Handle(request, CancellationToken.None);

		response.Should().BeEquivalentTo(new
		{
			Success = false,
			Message = $"Booking with id: {request.Id} not found"
		});
	}
}

