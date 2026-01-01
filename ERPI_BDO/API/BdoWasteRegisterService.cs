using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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


        public async Task<string> GetEupTokenAsync(string eupId)
        {
            try
            {
                var jwt = await _client.GenerateEupAccessTokenAsync(
                    new JwtRequest
                    {
                        ClientId = BdoConfig.ClientId,
                        ClientSecret = BdoConfig.ClientSecret,
                        EupId = eupId
                    });

                return jwt.AccessToken;
            }
            catch (ApiException ex)
                when (ex.StatusCode == 200 && !string.IsNullOrWhiteSpace(ex.Response))
            {
                // 🔴 BDO zwraca 200 + body, NSwag się myli
                var parsed = JsonSerializer.Deserialize<JwtAccessToken>(
                    ex.Response,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (parsed?.AccessToken == null)
                    throw;

                return parsed.AccessToken;
            }
        }


        /// <summary>
        /// Pobiera token sesji dla konkretnego miejsca (EUP)
        /// </summary>
        public async Task<string> GetEupTokenAsync_old_ok(string eupId) //_bład 200
        {
            var jwt = await _client.GenerateEupAccessTokenAsync(
                new JwtRequest
                {
                    ClientId = BdoConfig.ClientId,
                    ClientSecret = BdoConfig.ClientSecret,
                    EupId = eupId
                });
            return jwt.AccessToken;
        }

        /// <summary>
        /// Pobiera listę lokalizacji (EUP)
        /// </summary>
        public async Task<ICollection<CompanyEupDto>> GetEupListAsync()
        {
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
                return response.Items;
            }
            catch (ApiException ex) when (ex.StatusCode == 200 && !string.IsNullOrWhiteSpace(ex.Response))
            {
                var parsed = JsonSerializer.Deserialize<PaginatedPageCompanyEupDto>(
                    ex.Response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed?.Items ?? new List<CompanyEupDto>();
            }
        }

        public async Task<KpoReceiverListResponseDto> GetKpoReceiverListAsync(
            HttpClient eupClient,
            int pageIndex,
            int pageSize,
            Action<string> debug)
        {
            var url =
                "/WasteRegister/WasteTransferCard/v1/Kpo/sendercards/receiver" +
                $"?PaginationParameters.Page.Index={pageIndex}" +
                $"&PaginationParameters.Page.Size={pageSize}";

            // DEBUG – PEŁNY URL
            var fullUrl = new Uri(eupClient.BaseAddress!, url).ToString();
            debug($"KPO LIST FULL URL = {fullUrl}");


            var response = await eupClient.GetAsync(url);
            debug($"HTTP STATUS = {(int)response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            debug($"RESPONSE LENGTH = {json.Length}");


            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception(
                    $"KPO LIST ERROR\nHTTP {(int)response.StatusCode}\n<EMPTY RESPONSE>");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"KPO LIST ERROR\nHTTP {(int)response.StatusCode}\n{json}");
            }

            return JsonSerializer.Deserialize<KpoReceiverListResponseDto>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )!;
        }

        /// <summary>
        /// Pobiera szczegółowe dane dla pojedynczej karty KPO.
        /// Metoda próbuje kilku możliwych endpointów (transport/receive/approved/itd.)
        /// w kolejności praktycznej, bo KPO na liście może mieć różne statusy.
        /// </summary>
        public async Task<KpoDetailsDto?> GetKpoDetailsAsync(
            Guid kpoId,
            HttpClient eupClient,
            int companyType, // 0=SenderCompany,1=CarrierCompany,2=ReceiverCompany
            Action<string> debug)
        {
            debug?.Invoke($"Pobieranie detali KPO {kpoId} (CompanyType: {companyType})...");

            // Lista endpointów, które mogą zwracać szczegóły w zależności od statusu karty.
            var endpoints = new[]
            {
                "receiveconfirmed",      // Potwierdzenie przejęcia
                "transportconfirmation", // Potwierdzony transport
                "approved",              // Zatwierdzona
                "planned",               // Planowana
                "withdrawn",             // Wycofana
                "confirmationgenerated", // Wygenerowane potwierdzenie
                "rejected",              // Odrzucona
                "printingpage"           // Dane do wydruku (fallback)
            };

            foreach (var endpoint in endpoints)
            {
                try
                {
                    // Używamy ścieżki względnej względem BaseAddress klienta (jeżeli ustawiony)
                    var relativeUrl = $"/api/WasteRegister/WasteTransferCard/v1/Kpo/{endpoint}/card?KpoId={kpoId}&CompanyType={companyType}";

                    debug?.Invoke($"Próba endpointu: {endpoint} -> {relativeUrl}");

                    var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

                    var response = await eupClient.SendAsync(request);
                    var body = await response.Content.ReadAsStringAsync();

                    debug?.Invoke($"Status HTTP dla detali KPO {kpoId} (endpoint={endpoint}): {(int)response.StatusCode}");
                    debug?.Invoke($"Odpowiedź API dla detali KPO {kpoId} (endpoint={endpoint}): {body}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Niektóre endpointy (printingpage) mogą zwracać różne kształty JSON.
                        // Próbujemy zmapować na KpoDetailsDto (PropertyNameCaseInsensitive = true).
                        try
                        {
                            var details = JsonSerializer.Deserialize<KpoDetailsDto>(
                                body,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            debug?.Invoke($"Pobrane detale KPO (endpoint={endpoint}): CardNumber={details?.CardNumber}, WasteMass={details?.WasteMass}");
                            return details;
                        }
                        catch (Exception desEx)
                        {
                            debug?.Invoke($"Deserializacja odpowiedzi z endpoint={endpoint} nie powiodła się: {desEx.Message}");
                            // jeśli nie da się zdeserializować, spróbuj kolejny endpoint
                            continue;
                        }
                    }
                    else
                    {
                        // 404 oznacza, że dla danego endpointu karta nie istnieje — próbujemy następnego
                        if ((int)response.StatusCode == 404)
                        {
                            debug?.Invoke($"Karta nieznaleziona na endpoint={endpoint}, próbuję następnego.");
                            continue;
                        }

                        // dla innych statusów logujemy i przerywamy (np. 401, 403 mogą wymagać interwencji)
                        debug?.Invoke($"Błąd API podczas pobierania detali KPO {kpoId} (endpoint={endpoint}): {(int)response.StatusCode} - {body}");
                        // W zależności od potrzeb można przerwać lub kontynuować; kontynuujemy, aby spróbować kolejnych endpointów.
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    debug?.Invoke($"Wyjątek podczas próby pobrania detali KPO {kpoId} na jednym z endpointów: {ex.Message}");
                    // Spróbuj następnego endpointu
                    continue;
                }
            }

            debug?.Invoke($"Nie znaleziono detali KPO {kpoId} na żadnym znanym endpointzie.");
            return null;
        }
    }
}