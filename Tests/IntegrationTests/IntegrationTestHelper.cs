using AutoFixture;
using IntegrationTests.SpecimenBuilder;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;
public class IntegrationTestHelper : IClassFixture<IntegrationTestFactory<Program>>
{
	public Fixture Fixture { get; init; } 

	public readonly IntegrationTestFactory<Program> Factory;
	public readonly HttpClient HttpClient;

	public static Guid TenantId { get; } = Guid.NewGuid();

	public IntegrationTestHelper(IntegrationTestFactory<Program> factory)
	{
		Fixture = new Fixture();
		Fixture.Customizations.Add(new DateSpecimenBuilder());

		Factory = factory;
		HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false});
	}
}
