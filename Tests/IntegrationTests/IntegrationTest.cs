using AutoFixture;
using Booking.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;
public class IntegrationTestHelper : IClassFixture<IntegrationTestFactory<Program>>
{
	public Fixture Fixture { get; } = new();

	public readonly IntegrationTestFactory<Program> Factory;
	public readonly HttpClient HttpClient;

	public IntegrationTestHelper(IntegrationTestFactory<Program> factory)
	{
		Factory = factory;

		HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false});
	}
}

public class IntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureTestServices(services =>
		{
			var dbContextDescriptor =
				services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

			services.Remove(dbContextDescriptor);
			services.AddDbContext<ApplicationDbContext>((container, options) =>
			{
				options.UseInMemoryDatabase("DbName");
			});
		});

		builder.UseEnvironment("Test");
	}
}