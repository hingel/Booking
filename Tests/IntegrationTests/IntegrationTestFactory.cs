using Booking.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IntegrationTests;

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
				options.UseInMemoryDatabase("DbName")
				.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
			});

			services.AddAuthentication(TestAuthHandler.SchemaName)
			.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemaName, null);

			services.AddAuthorization(options => options.AddPolicy("Test", apb =>
			{
				apb.RequireAuthenticatedUser();
				apb.AuthenticationSchemes.Add(TestAuthHandler.SchemaName);
			})); //Detta nedan behövs inte egentligen.
		});

		builder.UseEnvironment("Test");
	}
}

public class TestAuthHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	public const string SchemaName = "TestScheme";

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new[] { new Claim(ClaimTypes.Name, "Test User"), new Claim("http://schemas.microsoft.com/identity/claims/tenantid", IntegrationTestHelper.TenantId.ToString()) };
		var identity = new ClaimsIdentity(claims, SchemaName);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, SchemaName);

		var result = AuthenticateResult.Success(ticket);
		return Task.FromResult(result);
	}
}