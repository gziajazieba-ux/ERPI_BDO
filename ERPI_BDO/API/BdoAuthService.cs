using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ERPI_BDO.Models;

namespace ERPI_BDO.Api;

public class BdoAuthService
{
    private readonly HttpClient _httpClient;

    public BdoAuthService()
    {
        _httpClient = new HttpClient();
    }

    // =========================================================
    // A. TOKEN SYSTEMOWY (client_credentials)
    // UŻYWANY DO:
    // - lista EUP
    // - KPO
    // - raporty
    // =========================================================

    public async Task<string> GetClientCredentialsTokenAsync(
        string clientId,
        string clientSecret)
    {
        var tokenEndpoint = $"{BdoConfig.BaseUrl}/connect/token";

        using var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        });

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"BDO AUTH ERROR: {(int)response.StatusCode}\n{body}");
        }

        var tokenResponse =
            Newtonsoft.Json.JsonConvert.DeserializeObject<BdoTokenResponse>(body);

        if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            throw new Exception("BDO AUTH ERROR: pusty access_token");

        return tokenResponse.AccessToken;
    }

  
    public async Task<BdoTokenResponse> GetTokenAsync(string clientId, string clientSecret)
    {
        // BDO Produkcja używa /api/OAuth/Token dla client_credentials
        string tokenUrl = "https://rejestr-bdo.mos.gov.pl/api/OAuth/Token";

        var dict = new Dictionary<string, string>
    {
        { "client_id", clientId },
        { "client_secret", clientSecret },
        { "grant_type", "client_credentials" }
    };

        var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(dict));
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Błąd autoryzacji: {response.StatusCode} - {body}");

        return JsonSerializer.Deserialize<BdoTokenResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    // =========================================================
    // B. LOGIN UŻYTKOWNIKA (authorization_code)
    // UŻYWANY DO:
    // - interaktywne logowanie
    // - operacje wymagające sesji użytkownika
    // =========================================================
    public async Task<BdoTokenResponse> ExchangeAuthorizationCodeAsync(
        string tokenUrl,
        string clientId,
        string clientSecret,
        string authorizationCode,
        string redirectUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

        var basicAuth = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", basicAuth);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", authorizationCode },
            { "redirect_uri", redirectUri }
        });

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"AUTH_CODE ERROR\nHTTP {(int)response.StatusCode}\n{body}");
        }

        return JsonSerializer.Deserialize<BdoTokenResponse>(body)!;
    }
}
