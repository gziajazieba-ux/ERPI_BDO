using ERPI_BDO.Models;
using ERPI_BDO.OpenApi.WasteRegister;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ERPI_BDO.Api
{
    public class BdoEupService
    {
        private readonly HttpClient _http;

        public BdoEupService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(BdoConfig.ApiBaseUrl)
            };
        }

        public async Task<List<CompanyEupDto>> GetEupsAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("Brak tokena użytkownika");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/WasteRegister/Company/v1/eup");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"EUP ERROR\nHTTP {(int)response.StatusCode}\n{json}");
            }

            return JsonSerializer.Deserialize<List<CompanyEupDto>>(json)!;
        }
    }
}
