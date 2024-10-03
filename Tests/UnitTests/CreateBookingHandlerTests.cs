using AutoFixture;
using Booking.Business.Commands.Handlers;
using Booking.Business.Repository;
using Booking.DataAccess.Models;
using NSubstitute;

namespace UnitTests
{
	public class CreateBookingHandlerTests
	{
		private Fixture fixture;
		private readonly IRepository repository;
		private readonly CreateBookingHandler subject;

		public CreateBookingHandlerTests()
        {
			fixture = new Fixture();

			repository = Substitute.For<IRepository>();
			subject = new CreateBookingHandler(repository);
        }

		[Fact]
		public void Execute_ValidBooking_AddsBookingToTable()
		{
			var bookedTable = fixture.Create<Table>();
			var booking = fixture.Build<Booking.DataAccess.Models.Booking>().With(b => b.CompanyId, bookedTable.CompanyId).Create();
			bookedTable.Bookings.Add(booking);

			repository.Tables(bookedTable.CompanyId).Returns([bookedTable]);

			var request = fixture.Build<CreateBooking>().With(b => b.CompanyId, bookedTable.CompanyId).Create();

			var response = subject.Execute(request);

			repository.Received(1).SaveChanges();
		}
	}
}