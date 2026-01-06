using ERPI_BDO.Models;
using ERPI_BDO.OpenApi.WasteRegister;
using ERPI_BDO.OpenApi.WasteRegister.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERPI_BDO.Api
{
    public class BdoWasteRegisterService
    {
        private readonly WasteRegisterApiClient _client;

        public BdoWasteRegisterService()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BdoConfig.BaseUrl)
            };
            _client = new WasteRegisterApiClient(httpClient);
        }

        // =========================================================
        // TOKEN EUP
        // =========================================================

        public async Task<string> GetEupTokenAsync(string eupId, Action<string>? debug = null)
        {
            debug?.Invoke($"[GetEupTokenAsync] START eupId={eupId}");

            try
            {
                var jwt = await _client.GenerateEupAccessTokenAsync(
                    new JwtRequest
                    {
                        ClientId = BdoConfig.ClientId,
                        ClientSecret = BdoConfig.ClientSecret,
                        EupId = eupId
                    });

                debug?.Invoke("[GetEupTokenAsync] OK token received");
                return jwt.AccessToken;
            }
            catch (ApiException ex)
                when (ex.StatusCode == 200 && !string.IsNullOrWhiteSpace(ex.Response))
            {
                debug?.Invoke("[GetEupTokenAsync] ApiException(200) – parsing body");

                var parsed = JsonSerializer.Deserialize<JwtAccessToken>(
                    ex.Response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed?.AccessToken == null)
                {
                    debug?.Invoke("[GetEupTokenAsync] ERROR parsed token is null");
                    throw;
                }

                debug?.Invoke("[GetEupTokenAsync] OK token parsed from body");
                return parsed.AccessToken;
            }
            catch (Exception ex)
            {
                debug?.Invoke($"[GetEupTokenAsync] ERROR {ex}");
                throw;
            }
        }


        // =========================================================
        // LISTA EUP
        // =========================================================

        public async Task<ICollection<CompanyEupDto>> GetEupListAsync(Action<string>? debug = null)
        {
            debug?.Invoke("[GetEupListAsync] START");

            try
            {
                var response = await _client.GetEupListAsync(
                    new EupRequest
                    {
                        ClientId = BdoConfig.ClientId,
                        ClientSecret = BdoConfig.ClientSecret,
                        PaginationParameters = new AuthPaginationParameters
                        {
                            Page = new Apage { Index = 0, Size = 50 },
                            Order = new Aorder { IsAscending = true }
                        }
                    });

                debug?.Invoke($"[GetEupListAsync] OK items={response.Items?.Count}");
                return response.Items;
            }
            catch (ApiException ex) when (ex.StatusCode == 200 && !string.IsNullOrWhiteSpace(ex.Response))
            {
                debug?.Invoke("[GetEupListAsync] ApiException(200) – parsing body");

                var parsed = JsonSerializer.Deserialize<PaginatedPageCompanyEupDto>(
                    ex.Response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                debug?.Invoke($"[GetEupListAsync] OK parsed items={parsed?.Items?.Count}");
                return parsed?.Items ?? new List<CompanyEupDto>();
            }
            catch (Exception ex)
            {
                debug?.Invoke($"[GetEupListAsync] ERROR {ex}");
                throw;
            }
        }


        // =========================================================
        // KPO DETAILS – KLUCZOWA METODA
        // =========================================================

        public async Task<List<KpoSearchItemDto>> GetKpoListAsync(string endpoint, object criteria, string token, Action<string>? debug = null)
        {
            debug?.Invoke($"[GetKpoList] START: {endpoint}");

            using var client = new HttpClient { BaseAddress = new Uri("https://api.bdo.mos.gov.pl") };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync(endpoint, criteria);
            if (!response.IsSuccessStatusCode)
            {
                debug?.Invoke($"[GetKpoList] ERROR: {response.StatusCode}");
                return new List<KpoSearchItemDto>();
            }

            string rawJson = await response.Content.ReadAsStringAsync();
            debug?.Invoke($"[RAW JSON] {rawJson}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<KpoSearchResponseDto>(rawJson, options);

            return result?.Items ?? new List<KpoSearchItemDto>();
        }


        // =========================================================
        // JSON HELPER
        // =========================================================

        private static string? GetString(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p) &&
                    p.ValueKind != JsonValueKind.Null)
                {
                    return p.ValueKind == JsonValueKind.String
                        ? p.GetString()
                        : p.ToString();
                }
            }
            return null;
        }
    }
}
