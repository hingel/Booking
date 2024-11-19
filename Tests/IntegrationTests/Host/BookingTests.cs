using AutoFixture;
using Booking.Business;
using Booking.Business.Mapping;
using Booking.Contract.Requests;
using Booking.Contract.Responses;
using Booking.DataAccess;
using Booking.DataAccess.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace IntegrationTests.Host;
public class BookingTests(IntegrationTestFactory<Program> factory) : IntegrationTestHelper(factory)
{
	[Fact]
	public async Task CreateBooking_DataPersisted()
	{
		var table = Fixture.Create<Table>() with { CompanyId = Fixture.Create<Guid>() };

		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Tables.Add(table);
		await dbContext.SaveChangesAsync();

		var bookingRequest = Fixture.Build<CreateBookingRequest>().With(b => b.CompanyId, table.CompanyId.ToString).Create();

		var response = await HttpClient.PostAsJsonAsync("/", bookingRequest);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		var test = dbContext.Tables.First().Bookings.First();
		
		test.Should().BeEquivalentTo(new
		{
			bookingRequest.Duration,
			NoOfPersons = bookingRequest.Persons,
			Contact = new
			{
				bookingRequest.Contact.Name,
				bookingRequest.Contact.Email,
				bookingRequest.Contact.PhoneNumber
			},
			TableId = table.Id,
			table.CompanyId
		});
	}

	[Fact]
	public async Task GetBookingById_ReturnsCorrect()
	{
		var table = Fixture.Create<Table>() with { CompanyId = Fixture.Create<Guid>() };
		var bookings = Fixture.Build<Booking.DataAccess.Models.Booking>().With(b => b.TableId, table.Id).CreateMany();

        foreach (var item in bookings)
        {
            table.Bookings.Add(item);
        }

		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Tables.Add(table);
		await dbContext.SaveChangesAsync();

		var response = await HttpClient.GetAsync($"/bookings/{bookings.Skip(1).First().Id}");
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<Result<BookingResponse>>();

		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Data.Should().BeEquivalentTo(bookings.Skip(1).Take(1).Select(b => b.ToContract()).First());
	}
}

