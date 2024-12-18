﻿using AutoFixture;
using Booking.Business;
using Booking.Business.Mapping;
using Booking.Contract.Requests;
using Booking.DataAccess;
using Booking.DataAccess.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace IntegrationTests.Host;

public class TablesTests(IntegrationTestFactory<Program> factory) : IntegrationTestHelper(factory)
{
	//Detta test behöver ta hand om anropet till HttpClienten 

	//[Fact]
	//public async Task AddTables_DataPersisted()
	//{
	//	var request = Fixture.Build<CreateTableRequest>().With(t => t.CompanyId, Fixture.Create<Guid>().ToString()).Create();
	//	var json = JsonSerializer.Serialize(request);
	//	var companyResponse = new
	//	{
	//		Id = request.CompanyId,
	//		Name = "Company Name",
	//		Address = "Company Address"
	//	};

	//	var jsonString = JsonSerializer.Serialize(companyResponse);

	//	var response = await HttpClient.PostAsync("/tables", new StringContent(json, Encoding.UTF8, "application/json"));

	//	response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	//	var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();

	//	result.Should().NotBeNull();
	//	result!.Success.Should().BeTrue();
	//	result.Message.Should().Be($"Table with name {request.Name} added");

	//	using var scope = Factory.Services.CreateScope();
	//	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	//}

	[Fact]
	public async Task GetTables_ResturnsTables()
	{
		var tables = Fixture.Build<Table>().With(t => t.CompanyId, IntegrationTestHelper.TenantId).CreateMany();

		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Tables.AddRange(tables);
		await dbContext.SaveChangesAsync();

		var response = await HttpClient.GetAsync("/tables");

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
		var result = await response.Content.ReadFromJsonAsync<Result<TableResponse[]>>();
		result.Should().NotBeNull();
		result!.Success.Should().BeTrue();
		result.Data!.Where(t => tables.Select(p => p.Id).Contains(t.Id)).Should().BeEquivalentTo(tables.Select(t => t.ToContract()));
	}
}