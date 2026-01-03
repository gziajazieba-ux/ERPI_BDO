using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ERPI_BDO.OpenApi.WasteRegister;
using ERPI_BDO.Models;
using ERPI_BDO.OpenApi.WasteRegister.Models;

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

        public async Task<KpoDetailsDto?> GetKpoDetailsAsync(
            Guid kpoId,
            HttpClient eupClient,
            int companyType, // 0=Sender, 1=Carrier, 2=Receiver
            Action<string> debug)
        {
            debug?.Invoke($"Pobieranie detali KPO {kpoId} (CompanyType={companyType})");

            var endpoints = new[]
            {
        "receiveconfirmed",
        "transportconfirmation",
        "approved",
        "planned",
        "withdrawn",
        "confirmationgenerated",
        "rejected",
        "printingpage"
    };

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var url =
                        $"/api/WasteRegister/WasteTransferCard/v1/Kpo/{endpoint}/card" +
                        $"?KpoId={kpoId}&CompanyType={companyType}";

                    debug?.Invoke($"DETAILS TRY: {url}");

                    var response = await eupClient.GetAsync(url);
                    var body = await response.Content.ReadAsStringAsync();

                    debug?.Invoke($"HTTP {(int)response.StatusCode}");
                    debug?.Invoke($"BODY LENGTH = {body.Length}");

                    if (!response.IsSuccessStatusCode)
                        continue;

                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;

                    var details = JsonSerializer.Deserialize<KpoDetailsDto>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (details == null)
                        continue;

                    // =====================================================
                    // ✅ MAPOWANIE company {} → CARRIER (zgodne z DTO)
                    // =====================================================
                    if (companyType == 1 && root.TryGetProperty("company", out var company))
                    {
                        details.CarrierIdentificationNumber =
                            GetString(company, "identificationNumber", "registryNumber");

                        details.CarrierNip =
                            GetString(company, "nip");

                        details.CarrierEuNip =
                            GetString(company, "euNip");

                        details.CarrierRegistrationNumber =
                            GetString(company, "registryNumber");

                        debug?.Invoke(
                            $"[CARRIER] Nip={details.CarrierNip}, Registry={details.CarrierRegistrationNumber}");
                    }

                    debug?.Invoke(
                        $"DETAILS OK ({endpoint}) CardNumber={details.CardNumber}, WasteMass={details.WasteMass}");

                    return details;
                }
                catch (Exception ex)
                {
                    debug?.Invoke($"DETAILS ERROR ({endpoint}): {ex.Message}");
                    continue;
                }
            }

            debug?.Invoke($"DETAILS NOT FOUND for KPO {kpoId}");
            return null;
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
