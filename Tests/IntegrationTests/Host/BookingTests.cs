using AutoFixture;
using Booking.Business;
using Booking.DataAccess;
using Booking.DataAccess.Models;
using Booking.Host.Contracts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace IntegrationTests.Host;

public class BookingTests(IntegrationTestFactory<Program> factory) : IntegrationTestHelper(factory)
{
	[Fact]
	public async Task AddTables_DataPersisted()
	{
		var request = Fixture.Build<CreateTableRequest>().With(t => t.CompanyId, Fixture.Create<Guid>().ToString()).Create();
		var json = JsonSerializer.Serialize(request);

		var response = await HttpClient.PostAsync("/tables", new StringContent(json, Encoding.UTF8, "application/json"));

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
		
		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Message.Should().Be($"Table with name {request.Name} added");
	}

	[Fact]
	public async Task GetTables_ResturnsTables()
	{
		var tables = Fixture.CreateMany<Table>();

		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Tables.AddRange(tables);
		await dbContext.SaveChangesAsync();

		var response = await HttpClient.GetAsync("/tables");

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<Result<Table[]>>();
		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Data.Should().BeEquivalentTo(tables);

	}

	[Fact]
	public async Task CreateBooking_DataPersisted()
	{
		var table = Fixture.Create<Table>() with { CompanyId = Fixture.Create<Guid>() };

		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Tables.Add(table);
		await dbContext.SaveChangesAsync();

		var bookingRequest = Fixture.Build<CreateBookingRequest>().With(b => b.CompanyId, table.CompanyId.ToString).Create();

		var response = await HttpClient.PostAsJsonAsync<CreateBookingRequest>("/", bookingRequest);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		dbContext.Tables.First().Bookings.First().Should().BeEquivalentTo(new
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
}