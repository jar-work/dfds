using dfds.domain;
using dfds.domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace dfds.infrastructure.Queries;

public class GetCountryFromCoordinateQuery: IGetCountryFromCoordinateQuery
{
    private const string ApiKey = "8ba3de439ad5465f81ed68f38122f386";

    public async Task<string> Query(Location location)
    {
        using var httpClient = new HttpClient();
        var requestUrl = $"https://api.opencagedata.com/geocode/v1/json?q={location.Latitude},{location.Longitude}&key={ApiKey}";
        var response = await httpClient.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to get country from coordinate: {response.ReasonPhrase}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseContent);

        var country = jsonResponse["results"][0]["components"]["country"];
        return (string)country;
    }
}