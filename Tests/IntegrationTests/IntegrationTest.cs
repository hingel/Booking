using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;

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