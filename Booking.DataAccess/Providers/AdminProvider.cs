using System.Net.Http.Json;

namespace Booking.DataAccess.Providers;

public class AdminProvider(HttpClient httpClient) : IAdminProvider
{
	public async Task<bool> VerifyCompany(Guid id)
	{
		var response = await httpClient.GetAsync($"/companies/{id}");
		response.EnsureSuccessStatusCode();

		var companyResponse = await response.Content.ReadFromJsonAsync<CompanyResponse>();

		return companyResponse != null && companyResponse.Id != Guid.Empty && companyResponse.Id == id; 
	}

	private record CompanyResponse(Guid Id, string Name, string? Address);
}
