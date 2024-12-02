using RichardSzalay.MockHttp;
using System.Text.Json;

namespace UnitTests;

public static class MockHttpExtension
{	
	public static MockedRequest RespondWithJson(this MockedRequest mockedRequest, object? responsObject) => 
		mockedRequest.Respond("application/json", JsonSerializer.Serialize(responsObject));	
}
