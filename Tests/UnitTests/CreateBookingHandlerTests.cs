using AutoFixture;
using Booking.Business.Commands.Handlers;
using Booking.Business.Repository;
using Booking.DataAccess.Models;
using NSubstitute;

namespace UnitTests
{
	public class CreateBookingHandlerTests : UnitTests
	{
		private readonly IRepository repository;
		private readonly CreateBookingHandler subject;

		public CreateBookingHandlerTests()
		{
			repository = Substitute.For<IRepository>();
			subject = new CreateBookingHandler(repository);
		}

		[Fact]
		public async Task Handle_ValidBooking_AddsBookingToTable()
		{
			var bookableTable = Fixture.Create<Table>();
			var bookingRequest = Fixture.Build<CreateBooking>().With(c => c.CompanyId, bookableTable.CompanyId).Create();

			repository.GetAvailableTables(bookingRequest).Returns([bookableTable]);

			var response = subject.Handle(bookingRequest, CancellationToken.None);

			await repository.Received(1).SaveChanges();
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

			var response = subject.Handle(bookingRequest, CancellationToken.None);
			await repository.Received(0).SaveChanges();
		}
	}
}