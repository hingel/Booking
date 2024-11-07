using AutoFixture;
using Booking.Business.Commands.Handlers;
using Booking.DataAccess.Models;
using FluentAssertions;

namespace UnitTests.Business
{
    public class CreateBookingHandlerTests : UnitTests
    {
        private readonly CreateBookingHandler subject;

        public CreateBookingHandlerTests()
        {
            subject = new CreateBookingHandler(DbContext);
        }

        [Fact]
        public async Task Handle_ValidBooking_AddsBookingToTable()
        {
            var bookableTable = Fixture.Create<Table>();
            var bookingRequest = Fixture.Build<CreateBooking>().With(c => c.CompanyId, bookableTable.CompanyId).Create();

            DbContext.Tables.Add(bookableTable);
            await DbContext.SaveChangesAsync();

            var response = await subject.Handle(bookingRequest, CancellationToken.None);
            response.Should().BeEquivalentTo(new
            {
                Success = true,
                Message = "Booking added",
            });
        }

        [Fact]
        public async Task Handle_InValidBooking_Returns()
        {
            var bookedTable = Fixture.Create<Table>();
            var booking = Fixture.Build<Booking.DataAccess.Models.Booking>()
                .With(b => b.CompanyId, bookedTable.CompanyId)
                .Create();

            bookedTable.Bookings.Add(booking);

            var bookingRequest = Fixture.Build<CreateBooking>()
                .With(c => c.DateTime, booking.DateTime)
                .With(c => c.CompanyId, bookedTable.CompanyId)
                .Create();

            var response = await subject.Handle(bookingRequest, CancellationToken.None);
            response.Should().BeEquivalentTo(new
            {
                Success = false,
                Message = "Booking not added"
            });
        }
    }
}