using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ERPI_BDO.Api;

public class BdoApiClient
{
    private readonly HttpClient _httpClient;

    public BdoApiClient(string baseUrl, string accessToken)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        return await _httpClient.GetFromJsonAsync<T>(endpoint);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, object data)
    {
        return await _httpClient.PostAsJsonAsync(endpoint, data);
    }
}

