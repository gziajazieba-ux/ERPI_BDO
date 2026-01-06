using ERPI_BDO.Api;
using ERPI_BDO.Models;
using ERPI_BDO.OpenApi.WasteRegister; // Tu nadal trzymamy DTO z NSwag dla listy, np. CompanyEupDto, KpoSearchResult
using ERPI_BDO.OpenApi.WasteRegister.Models; // <- DODANE: tu jest KpoDetailsDto
using ERPI_BDO.Portal;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System; // dla DateTime itp.
using System.Collections.Generic; // Dodaj to
using System.Globalization; // dla parsowania daty/czasu
using System.Linq; // Dodaj to
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms; // używane typy WinForms

namespace ERPI_BDO
{
    public partial class FormGlowne : Form
    {
        // POLA (zmienne globalne)
        private CompanyEupDto? _selectedEup;
        private CoreWebView2Environment? _webEnv;
        private HttpClient? _apiClient;
        private string? _eupToken; // <--- To musi być tutaj!
        // Zmień inicjalizację, aby używać BdoWasteRegisterService z folderu Api
        private readonly Api.BdoWasteRegisterService _bdoService = new Api.BdoWasteRegisterService();
        private Guid? _selectedEupId;
        private BdoEupContext? _eupContext;

        // Dodane pola klasy (w sekcji pól klasy FormGlowne)
        private System.Windows.Forms.Timer _kpoLoadingTimer;
        private string _kpoLoadingBaseText = "";
        private int _kpoLoadingDots = 0;
        private const int KpoLoadingMaxDots = 3;

        // Zastąp istniejący konstruktor tym (inicjalizacja timera)
        public FormGlowne()
        {
            InitializeComponent();

            _kpoLoadingTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _kpoLoadingTimer.Tick += KpoLoadingTimer_Tick;
        }

        private void FormGlowne_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Gotowy";
            lblWybranyEup.Text = "Brak wybranego EUP";
            System.Net.ServicePointManager.SecurityProtocol =
            System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;
        }
        private static string? GetString(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p) && p.ValueKind != JsonValueKind.Null)
                {
                    if (p.ValueKind == JsonValueKind.String)
                        return p.GetString();

                    return p.ToString();
                }
            }
            return null;
        }

        private static Guid? GetGuid(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p))
                {
                    // GUID jako string (najczęstszy przypadek)
                    if (p.ValueKind == JsonValueKind.String &&
                        Guid.TryParse(p.GetString(), out var g))
                    {
                        return g;
                    }

                    // czasem BDO zwraca guid jako obiekt (rzadkie, ale bywa)
                    if (p.ValueKind == JsonValueKind.Object &&
                        Guid.TryParse(p.ToString(), out g))
                    {
                        return g;
                    }
                }
            }
            return null;
        }

        private static int? GetInt(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p))
                {
                    if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var i))
                        return i;

                    if (p.ValueKind == JsonValueKind.String &&
                        int.TryParse(p.GetString(), out i))
                        return i;
                }
            }
            return null;
        }

        private static decimal? GetDecimal(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p))
                {
                    if (p.ValueKind == JsonValueKind.Number &&
                        p.TryGetDecimal(out var d))
                        return d;

                    if (p.ValueKind == JsonValueKind.String &&
                        decimal.TryParse(
                            p.GetString(),
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out d))
                        return d;
                }
            }
            return null;
        }

        private static bool? GetBool(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p))
                {
                    if (p.ValueKind == JsonValueKind.True) return true;
                    if (p.ValueKind == JsonValueKind.False) return false;

                    if (p.ValueKind == JsonValueKind.String &&
                        bool.TryParse(p.GetString(), out var b))
                        return b;
                }
            }
            return null;
        }

        private static DateTime? GetDate(JsonElement el, params string[] names)
        {
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var p) &&
                    p.ValueKind == JsonValueKind.String &&
                    DateTime.TryParse(p.GetString(), out var dt))
                {
                    return dt;
                }
            }
            return null;
        }

        private async void HideSkipLink()
        {
            if (webViewLogin.CoreWebView2 == null)
                return;

            var css = @"
        a[href='#main'],
        .skip-link,
        .sr-only-focusable {
            display: none !important;
        }
    ";

            var script = $@"
        var style = document.createElement('style');
        style.innerHTML = `{css}`;
        document.head.appendChild(style);
    ";

            await webViewLogin.ExecuteScriptAsync(script);
        }

        private void OnUserLoggedOut()
        {
            btnZaloguj.Enabled = true;
            _apiClient = null;

            lblStatus.Text = "Sesja wygasła lub użytkownik się wylogował.";
            lblStatus.ForeColor = Color.DarkOrange;

            tabControl1.SelectedTab = tabPage1;
        }
        private async void WebView_NavigationCompleted(
            object? sender,
            CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
                return;

            HideSkipLink();

            await CheckLoginStateAsync();
        }

        private async Task CheckLoginStateAsync()
        {
            if (webViewLogin.CoreWebView2 == null)
                return;

            // sprawdzamy czy istnieje element "Wyloguj"
            var script = @"
        (function() {
            const links = Array.from(document.querySelectorAll('a, button'));
            return links.some(e => e.innerText && e.innerText.includes('Wyloguj'));
        })();
    ";

            var result = await webViewLogin.ExecuteScriptAsync(script);

            if (result == "true")
                OnUserLoggedIn();
            else
                OnUserLoggedOut();
        }
        // Ta metoda powinna zostać wywołana, gdy WebView2 wykryje udane logowanie
        private async void OnUserLoggedIn()
        {
            try
            {
                lblStatus.Text = "Logowanie udane. Pobieranie listy EUP...";
                lblStatus.ForeColor = Color.Blue;

                // 2. Pobranie listy EUP automatycznie
                var eups = await _bdoService.GetEupListAsync();

                if (eups != null && eups.Any())
                {
                    // Wypełnienie Grida w Tab2
                    dgvEup.DataSource = eups.ToList();

                    // Przełączenie na Tab2 automatycznie
                    tabControl1.SelectedTab = tabPage2;

                    lblStatus.Text = $"Pobrano {eups.Count} lokalizacji. Wybierz właściwy EUP.";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    lblStatus.Text = "Nie znaleziono lokalizacji EUP.";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Błąd automatycznego pobierania EUP.";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Szczegóły: {ex.Message}");
            }
        }

        private async Task InitWebViewAsync()
        {
            _webEnv = await CoreWebView2Environment.CreateAsync(
                null,
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ERPI_BDO_WebView"));

            await webViewLogin.EnsureCoreWebView2Async(_webEnv);

            webViewLogin.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;

            webViewLogin.CoreWebView2.Navigate(
                "https://rejestr-bdo.mos.gov.pl/");
        }
        private async void btnPobierzKpo_Click(object sender, EventArgs e)
        {
            DebugLogger.Add("=== btnPobierzKpo_Click (STABLE – 3 SEARCH ENDPOINTS) ===");
            StartKpoLoading("Czytam karty");

            try
            {
                bool wgDatTransportu = checkBoxDatyTransportu.Checked;
                if (_eupContext == null || string.IsNullOrEmpty(_eupToken))
                {
                    lblStatus.Text = "Błąd: Brakuje kontekstu EUP.";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                if (_apiClient == null)
                {
                    lblStatus.Text = "Błąd: Klient API nie jest gotowy.";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                // =========================================================
                // 1) RECEIVER
                // =========================================================
                var receiverCriteria = new
                {
                    PaginationParameters = new
                    {
                        Order = new { IsAscending = false, OrderColumn = "ReceiveConfirmationTime" },
                        Page = new { Index = 0, Size = 50 }
                    },
                    Year = dtKpoOd.Value.Year,

                    SearchInCarriers = true,
                    SearchInSenders = true,

                    ReceiveConfirmationDateRange = !wgDatTransportu,
                    ReceiveConfirmationDateFrom = wgDatTransportu ? null : dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    ReceiveConfirmationDateTo = wgDatTransportu ? null : dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z"),

                    TransportDateRange = wgDatTransportu,
                    TransportDateFrom = wgDatTransportu ? dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z") : null,
                    TransportDateTo = wgDatTransportu ? dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z") : null

                };


                //var receiverList = await PostSearchAndBuildFullDetailsListAsync(
                //    "/api/WasteRegister/WasteTransferCard/v1/Kpo/receiver/search",
                //    receiverCriteria,
                //    companyType: 2
                //);
                //var receiverList = BuildListFromSearch(searchReceiverResponse, CompanyType.Receiver);
                //var receiverList = searchReceiverResponse.Items;
                //var receiverList = BuildListFromSearch(receiverResponse, CompanyType.Receiver);
                //var receiverList = await PostSearchAndBuildFullDetailsListAsync(
                // "/api/WasteRegister/WasteTransferCard/v1/Kpo/receiver/search",
                //receiverCriteria,
                //companyType: 2
                //);
                var receiverList = await PostSearchOnlyAsync(
                "/api/WasteRegister/WasteTransferCard/v1/Kpo/receiver/search",
                receiverCriteria
                );

                // =========================================================
                // 2) SENDER
                // =========================================================
                var senderCriteria = new
                {
                    PaginationParameters = new
                    {
                        Order = new { IsAscending = false, OrderColumn = "WasteTransferCardNumber" },
                        Page = new { Index = 0, Size = 50 }
                    },
                    Year = dtKpoOd.Value.Year,

                    SearchInCarriers = true,
                    SearchInReceivers = true,

                    TransportDateRange = wgDatTransportu,
                    TransportDateFrom = wgDatTransportu ? dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z") : null,
                    TransportDateTo = wgDatTransportu ? dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z") : null,

                    ReceiveConfirmationDateRange = !wgDatTransportu,
                    ReceiveConfirmationDateFrom = wgDatTransportu ? null : dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    ReceiveConfirmationDateTo = wgDatTransportu ? null : dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z")

                };


                //var senderList = await PostSearchAndBuildFullDetailsListAsync(
                //    "/api/WasteRegister/WasteTransferCard/v1/Kpo/sender/search",
                //    senderCriteria,
                //    companyType: 0
                //);
                //var senderList = BuildListFromSearch(searchSenderResponse, CompanyType.Sender);
                var senderList = await PostSearchOnlyAsync(
                "/api/WasteRegister/WasteTransferCard/v1/Kpo/sender/search",
                senderCriteria
                );

                // =========================================================
                // 3) CARRIER
                // =========================================================
                var transportCriteria = new
                {
                    PaginationParameters = new
                    {
                        Order = new { IsAscending = false, OrderColumn = "PlannedTransportTime" },
                        Page = new { Index = 0, Size = 50 }
                    },
                    Year = dtKpoOd.Value.Year,

                    SearchInSenders = true,
                    SearchInReceivers = true,

                    TransportDateRange = wgDatTransportu,
                    TransportDateFrom = wgDatTransportu ? dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z") : null,
                    TransportDateTo = wgDatTransportu ? dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z") : null,

                    ReceiveConfirmationDateRange = !wgDatTransportu,
                    ReceiveConfirmationDateFrom = wgDatTransportu ? null : dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    ReceiveConfirmationDateTo = wgDatTransportu ? null : dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z")

                };


                //var transportList = await PostSearchAndBuildFullDetailsListAsync(
                //    "/api/WasteRegister/WasteTransferCard/v1/Kpo/carrier/search",
                //    transportCriteria,
                //    companyType: 1
                //);
                //var transportList = BuildListFromSearch(searchTransportResponse, CompanyType.Carrier);
                var transportList = await PostSearchOnlyAsync(
                "/api/WasteRegister/WasteTransferCard/v1/Kpo/carrier/search",
                transportCriteria
                );

                // =========================================================
                // BIND GRIDÓW – PO PEŁNYM ODCZYCIE LIST + DETAILS
                // =========================================================
                this.Invoke((MethodInvoker)delegate
                {
                    DebugLogger.Add("=== BINDING GRIDS (FINAL) ===");

                    // RECEIVER
                    dgvKpo.AutoGenerateColumns = true;
                    dgvKpo.DataSource = null;
                    dgvKpo.DataSource = receiverList;

                    // SENDER
                    dgvKpoSender.AutoGenerateColumns = true;
                    dgvKpoSender.DataSource = null;
                    dgvKpoSender.DataSource = senderList;

                    // TRANSPORT
                    dgvKpoTransport.AutoGenerateColumns = true;
                    dgvKpoTransport.DataSource = null;
                    dgvKpoTransport.DataSource = transportList;

                    lblStatus.Text =
                        $"Pobrano: receiver={receiverList.Count}, sender={senderList.Count}, transport={transportList.Count}";
                    lblStatus.ForeColor = Color.Green;
                });

            }
            catch (Exception ex)
            {
                DebugLogger.Add(ex.ToString());
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopKpoLoading();
            }
        }




        // Pomocnik: wysyła POST search, deserializuje wynik i dla każdego item pobiera szczegóły oraz mapuje na KpoReceiverFullDetailsDto
        private void ConfigureDgvKpoColumns(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();

            // Pełna lista kolumn odpowiadająca właściwościom KpoReceiverFullDetailsDto.
            var cols = new (string Property, string Header)[]
            {
                ("KpoId","kpoId"),
                ("Year","year"),
                ("CardNumber","cardNumber"),
                ("CardStatus","cardStatus"),
                ("CardStatusId","cardStatusId"),
                ("CardStatusCodeName","cardStatusCodeName"),

                ("SenderCompanyId","senderCompanyId"),
                ("SenderEupId","senderEupId"),
                ("SenderCompanyName","senderCompanyName"),
                ("SenderFirstNameAndLastName","senderFirstNameAndLastName"),
                ("SenderName","senderName"),

                ("ReceiverCompanyId","receiverCompanyId"),
                ("ReceiverEupId","receiverEupId"),
                ("ReceiverCompanyName","receiverCompanyName"),
                ("ReceiverFirstAndLastName","receiverFirstNameAndLastName"),
                ("ReceiverName","receiverName"),

                ("CarrierCompanyId","carrierCompanyId"),
                ("CarrierEupId","carrierEupId"),
                ("CarrierCompanyName","carrierCompanyName"),

                ("WasteCodeId","wasteCodeId"),
                ("WasteCode","wasteCode"),
                ("WasteCodeDescription","wasteCodeDescription"),
                ("WasteCodeExtended","wasteCodeExtended"),
                ("WasteCodeExtendedDescription","wasteCodeExtendedDescription"),
                ("WasteMass","wasteMass"),
                ("CorrectedWasteMass","correctedWasteMass"),
                ("RevisedWasteMass","revisedWasteMass"),
                ("RevisedWasteCodeId","revisedWasteCodeId"),
                ("Quantity","wasteMassDisplayed"),

                ("IsRevised","isRevised"),
                ("RevisedAt","revisedAt"),
                ("RevisedBy","revisedBy"),

                ("PlannedTransportTime","plannedTransportTime"),
                ("RealTransportTime","realTransportTime"),
                ("ReceiveConfirmationTime","receiveConfirmationTime"),
                ("TransportConfirmationTime","transportConfirmationTime"),
                ("CardApprovalTime","cardApprovalTime"),
                ("GeneratingConfirmationTime","generatingConfirmationTime"),
                ("CardRejectionTime","cardRejectionTime"),

                ("ApprovalUser","approvalUser"),
                ("TransportConfirmationUser","transportConfirmationUser"),
                ("GeneratingConfirmationUser","generatingConfirmationUser"),
                ("ReceiveConfirmationUser","receiveConfirmationUser"),
                ("RejectedByUser","rejectedByUser"),

                ("VehicleRegNumber","vehicleRegNumber"),
                ("CertificateNumberAndBoxNumbers","certificateNumberAndBoxNumbers"),
                ("AdditionalInfo","additionalInfo"),
                ("IsWasteGenerating","isWasteGenerating"),
                ("WasteGeneratingAdditionalInfo","wasteGeneratingAdditionalInfo"),
                ("WasteGeneratedTerytPk","wasteGeneratedTerytPk"),
                ("WasteProcessId","wasteProcessId"),
                ("HazardousWasteReclassification","hazardousWasteReclassification"),
                ("HazardousWasteReclassificationDescription","hazardousWasteReclassificationDescription"),
                ("Remarks","remarks"),

                ("CreatedDate","createdDate"),
                ("IdentificationNumber","identificationNumber"),
                ("Nip","nip"),
                ("AddressHtml","addressHtml"),
                ("PostalCode","postalCode"),
                ("CountryName","countryname"),
                ("Locality","locality"),
                ("Street","street"),
                ("BuildingNumber","buildingnumber"),
                ("LocalNumber","localnumber"),
                ("AdditionalInfoForUi","additionalInfoUi")
            };

            foreach (var (prop, header) in cols)
            {
                var c = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = prop,
                    Name = prop,
                    HeaderText = header,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                dgv.Columns.Add(c);
            }
        }

        private async Task<List<KpoReceiverListItemDto>> PostSearchOnlyAsyncold1(
            string url,
            object criteria)
        {
            DebugLogger.Add($"=== SEARCH ONLY START ({url}) ===");

            if (_apiClient == null)
                throw new InvalidOperationException("API client is not initialized.");

            // 🔑 KLUCZOWA LINIA – object → HttpContent
            var content = JsonContent.Create(criteria);

            var httpResponse = await _apiClient.PostAsync(url, content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                DebugLogger.Add(
                    $"SEARCH ONLY HTTP ERROR: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                return new List<KpoReceiverListItemDto>();
            }

            var searchResponse =
                await httpResponse.Content.ReadFromJsonAsync<KpoReceiverListResponseDto>();

            DebugLogger.Add(
                $"=== SEARCH ONLY END | COUNT={searchResponse?.Items?.Count ?? 0} ===");

            return searchResponse?.Items ?? new List<KpoReceiverListItemDto>();
        }

        // ZMIANA: Zwracamy nową, czystą klasę KpoSearchItemDto
        private async Task<List<KpoSearchItemDto>> PostSearchOnlyAsync(string url, object criteria)
        {
            DebugLogger.Add($"=== [DEBUG] WYWOŁANIE SEARCH: {url} ===");

            if (_apiClient == null) throw new InvalidOperationException("Klient API nie jest zainicjalizowany.");

            var response = await _apiClient.PostAsJsonAsync(url, criteria);

            if (!response.IsSuccessStatusCode)
            {
                DebugLogger.Add($"[BŁĄD API] Status: {response.StatusCode} na url: {url}");
                return new List<KpoSearchItemDto>();
            }

            // 1. POBIERAMY SUROWY TEKST - To jest nasz "jedyny punkt prawdy"
            string rawJson = await response.Content.ReadAsStringAsync();

            // 2. LOGUJEMY SUROWY JSON (pierwsze 2000 znaków), abyś mógł go skopiować do analizy
            DebugLogger.Add($"[RAW JSON START] {url}");
            DebugLogger.Add(rawJson.Length > 2000 ? rawJson.Substring(0, 2000) : rawJson);
            DebugLogger.Add($"[RAW JSON END]");

            // 3. DESERIALIZACJA
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var searchResponse = JsonSerializer.Deserialize<KpoSearchResponseDto>(rawJson, options);

            return searchResponse?.Items ?? new List<KpoSearchItemDto>();
        }




        /*
        private async Task<List<KpoReceiverFullDetailsDto>>
 PostSearchAndBuildFullDetailsListAsync(
     string searchUrl,
     object searchCriteria,
     int companyType // 0=Sender, 1=Carrier, 2=Receiver
 )
        {
            DebugLogger.Add("=== PostSearchAndBuildFullDetailsListAsync START ===");

            var result = new List<KpoReceiverFullDetailsDto>();

            using var eupClient = new HttpClient
            {
                BaseAddress = new Uri(BdoConfig.BaseUrl)
            };
            eupClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _eupToken);

            // =====================================================
            // SEARCH
            // =====================================================
            var payloadJson = JsonSerializer.Serialize(searchCriteria);

            var response = await eupClient.PostAsync(
                searchUrl,
                new StringContent(payloadJson, Encoding.UTF8, "application/json")
            );

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return result;

            using var doc = JsonDocument.Parse(body);

            if (!doc.RootElement.TryGetProperty("items", out var items) ||
                items.ValueKind != JsonValueKind.Array)
                return result;

            // =====================================================
            // LIST → DTO
            // =====================================================
            foreach (var item in items.EnumerateArray())
            {
                var kpoId = GetGuid(item, "kpoId");
                if (kpoId == null)
                    continue;

                var dto = new KpoReceiverFullDetailsDto
                {
                    // ===== IDENTYFIKACJA =====
                    KpoId = kpoId,
                    Year = GetInt(item, "year"),
                    CardNumber = GetString(item, "cardNumber"),

                    // ===== STATUS =====
                    CardStatus = GetString(item, "cardStatus"),
                    CardStatusCodeName = GetString(item, "cardStatusCodeName"),

                    // ===== ODPAD (LIST) =====
                    WasteCodeId = GetInt(item, "wasteCodeId"),
                    WasteCode = GetString(item, "wasteCode"),
                    WasteCodeDescription = GetString(item, "wasteCodeDescription"),

                    DeclaredWasteMass =
                        GetDecimal(item, "quantity") is decimal q
                        ? (double?)q
                        : GetDecimal(item, "wasteMass") is decimal wm
                        ? (double?)wm
                        : null,


                    WasteCodeExtended = GetBool(item, "wasteCodeExtended"),
                    WasteCodeExtendedDescription =
                        GetString(item, "wasteCodeExtendedDescription"),
                    HazardousWasteReclassification =
                        GetBool(item, "hazardousWasteReclassification"),
                    HazardousWasteReclassificationDescription =
                        GetString(item, "hazardousWasteReclassificationDescription"),

                    // ===== SENDER (LIST) =====
                    SenderCompanyId = GetGuid(item, "senderCompanyId"),
                    SenderCompanyName = GetString(item, "senderCompanyName"),
                    SenderEupId = GetGuid(item, "senderEupId"),

                    // ===== RECEIVER (LIST) =====
                    ReceiverCompanyId = GetGuid(item, "receiverCompanyId"),
                    ReceiverCompanyName =
                        GetString(item, "receiverNameOrFirstNameAndLastName"),
                    ReceiverNip = GetString(item, "receiverNip"),
                    ReceiverNipEu = GetString(item, "receiverNipEu"),
                    ReceiverEupId = GetGuid(item, "receiverEupId"),
                    ReceiverEupNumber = GetString(item, "receiverEupNumber"),
                    ReceiverEupName = GetString(item, "receiverEupName"),
                    ReceiverEupAddress = GetString(item, "receiverEupAddress"),

                    // ===== CARRIER (LIST) =====
                    CarrierCompanyId = GetGuid(item, "carrierCompanyId"),
                    CarrierCompanyName = GetString(item, "carrierCompanyName"),
                    CarrierNip = GetString(item, "carrierNip"),
                    CarrierNipEu = GetString(item, "carrierNipEu"),
                    VehicleRegNumber = GetString(item, "vehicleRegNumber"),

                    // ===== DATY (LIST) =====
                    PlannedTransportTime =
                        item.TryGetProperty("plannedTransportTime", out var ptt)
                            ? ptt.GetDateTimeOffset()
                            : null,

                    RealTransportTime =
                        item.TryGetProperty("realTransportTime", out var rtt)
                            ? rtt.GetDateTimeOffset()
                            : null,

                    CardApprovalTime =
                        item.TryGetProperty("cardApprovalTime", out var cat)
                            ? cat.GetDateTimeOffset()
                            : null,

                    InstallationName = GetString(item, "installationName"),
                    WasteProcessId = GetInt(item, "wasteProcessId"),
                    Remarks = GetString(item, "remarks"),
                    AdditionalInfo = GetString(item, "additionalInfo")
                };

                // =====================================================
                // DETAILS (bez zmiany sposobu pobierania)
                // =====================================================
                var details = await _bdoService.GetKpoDetailsAsync(
                    kpoId.Value,
                    eupClient,
                    companyType,
                    DebugLogger.Add
                );

                if (details != null)
                {
                    dto.CardStatusId = details.CardStatusId;

                    dto.RevisedWasteMass = details.RevisedWasteMass;
                    dto.CorrectedWasteMass = details.CorrectedWasteMass;

                    // WasteMass z DETAILS:
                    // - jeśli brak korekt → traktujemy jako DECLARED
                    // - jeśli są korekty → traktujemy jako RECEIVED
                    if (details.WasteMass != null)
                    {
                        if (dto.RevisedWasteMass == null && dto.CorrectedWasteMass == null)
                        {
                            dto.DeclaredWasteMass ??= details.WasteMass;
                        }
                        else
                        {
                            dto.ReceivedWasteMass = details.WasteMass;
                        }
                    }



                    dto.ReceiveConfirmationTime =
                        details.ReceiveConfirmationTime ??
                        details.ReceiveConfirmationTimeFromKpo;

                    dto.TransportConfirmationTime =
                        details.TransportConfirmationTime;

                    dto.ApprovalUser = details.ApprovalUser;
                    dto.WithdrawnByUser = details.WithdrawnByUser;
                    dto.CertificateNumberAndBoxNumbers =
                        details.CertificateNumberAndBoxNumbers;
                }
                dto.EffectiveWasteMass =
    dto.CorrectedWasteMass
    ?? dto.RevisedWasteMass
    ?? dto.ReceivedWasteMass
    ?? dto.DeclaredWasteMass;

                result.Add(dto);
            }

            DebugLogger.Add(
                $"=== PostSearchAndBuildFullDetailsListAsync END | COUNT={result.Count} ===");

            return result;
        }
        */

        private async void btnZaloguj_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Otwieranie logowania BDO...";
            lblStatus.ForeColor = Color.Black;

            await InitWebViewAsync();
        }
        private async void dgvEup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DebugLogger.Add("=== dgvEup_CellDoubleClick START ===");

            try
            {
                if (e.RowIndex < 0)
                {
                    DebugLogger.Add("RowIndex < 0 – ignoruję");
                    return;
                }

                if (dgvEup.Rows[e.RowIndex].DataBoundItem is not CompanyEupDto eup)
                {
                    DebugLogger.Add("DataBoundItem != CompanyEupDto");
                    return;
                }

                DebugLogger.Add("Wybrany EUP:");
                DebugLogger.Add($"  EupId      = {eup.EupId}");
                DebugLogger.Add($"  CompanyId  = {eup.CompanyId}");
                DebugLogger.Add($"  Name       = {eup.Name}");

                // =========================
                // ZAPAMIĘTANIE WYBORU
                // =========================
                _selectedEup = eup;
                _selectedEupId = eup.EupId;

                // =========================
                // TOKEN EUP
                // =========================
                DebugLogger.Add("Pobieranie tokena EUP – start");

                if (eup.EupId == null)
                    throw new Exception("EUP EupId == null");

                var wasteRegisterService = new Api.BdoWasteRegisterService(); // Używamy Api.BdoWasteRegisterService

                var eupToken = await wasteRegisterService.GetEupTokenAsync(
                    eup.EupId.Value.ToString()
                );

                if (string.IsNullOrWhiteSpace(eupToken))
                    throw new Exception("Token EUP jest pusty");

                _eupToken = eupToken;

                DebugLogger.Add("Token EUP – OK");
                DebugLogger.Add($"Token EUP length = {eupToken.Length}");

                // =========================
                // KLIENT API POD EUP
                // =========================
                DebugLogger.Add("Budowanie HttpClient pod EUP");

                // Inicjalizacja HttpClientHandler z wyłączonym proxy
                var handler = new HttpClientHandler { UseProxy = false };
                var eupApiClient = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(60),
                    BaseAddress = new Uri("https://rejestr-bdo.mos.gov.pl/")
                };

                eupApiClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", eupToken);

                eupApiClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

                // Dodajemy kontekstowe nagłówki EUP/Company/Year – potrzebne dla niektórych GET
                eupApiClient.DefaultRequestHeaders.Add("X-Eup-Id", eup.EupId.Value.ToString());
                eupApiClient.DefaultRequestHeaders.Add("X-Company-Id", eup.CompanyId.Value.ToString());
                eupApiClient.DefaultRequestHeaders.Add("X-Year-Context", DateTime.Now.Year.ToString());

                eupApiClient.DefaultRequestHeaders.Accept.Clear();
                eupApiClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );

                _apiClient = eupApiClient;

                DebugLogger.Add("HttpClient EUP – OK");

                // =========================
                // KONTEXT EUP
                // =========================
                if (eup.CompanyId == null)
                    throw new Exception("EUP CompanyId == null");

                _eupContext = new BdoEupContext
                {
                    EupId = eup.EupId.Value,
                    CompanyId = eup.CompanyId.Value,
                    Year = DateTime.Now.Year,
                    EupToken = eupToken,
                    ApiClient = eupApiClient
                };

                DebugLogger.Add("EUP CONTEXT UTWORZONY");
                DebugLogger.Add($"CTX EupId = {_eupContext.EupId}");

                // =========================
                // UI
                // =========================
                BeginInvoke(new Action(() =>
                {
                    lblStatus.Text = "Wybrano EUP – token EUP OK";
                    lblStatus.ForeColor = Color.Green;

                    lblWybranyEup.Text = $"{eup.Name} [{eup.EupId}]";

                    statusStrip1.Refresh();
                    tabControl1.SelectedTab = tabPage3;
                }));

                DebugLogger.Add("Przełączono do Tab3");
                DebugLogger.Add("=== dgvEup_CellDoubleClick END ===");
            }
            catch (Exception ex)
            {
                DebugLogger.Add("!!! dgvEup_CellDoubleClick ERROR !!!");
                DebugLogger.Add(ex.ToString());

                BeginInvoke(new Action(() =>
                {
                    lblStatus.Text = "Błąd wyboru EUP";
                    lblStatus.ForeColor = Color.Red;
                }));
            }
        }


        private void debugToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var debugForm = new FormDebugText(
    "DEBUG",
    DebugLogger.GetText()
);

            debugForm.Show();
        }

        // Dodane: metody kontrolujące animację/loading (w klasie FormGlowne)
        private void KpoLoadingTimer_Tick(object? sender, EventArgs e)
        {
            _kpoLoadingDots = (_kpoLoadingDots + 1) % (KpoLoadingMaxDots + 1);
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    toolStripStatusLabelKpo.Text = _kpoLoadingBaseText + new string('.', _kpoLoadingDots);
                }));
            }
            else
            {
                toolStripStatusLabelKpo.Text = _kpoLoadingBaseText + new string('.', _kpoLoadingDots);
            }
        }

        private void StartKpoLoading(string baseText)
        {
            _kpoLoadingBaseText = baseText ?? "";
            _kpoLoadingDots = 0;
            toolStripStatusLabelKpo.Text = _kpoLoadingBaseText;
            _kpoLoadingTimer.Start();
            btnPobierzKpo.Enabled = false;
        }

        private void StopKpoLoading()
        {
            _kpoLoadingTimer.Stop();
            toolStripStatusLabelKpo.Text = "";
            btnPobierzKpo.Enabled = true;
            _kpoLoadingBaseText = "";
            _kpoLoadingDots = 0;
        }

        private List<KpoReceiverFullDetailsDto> BuildListFromSearch(
    KpoReceiverListResponseDto response,
    CompanyType companyType)
        {
            var result = new List<KpoReceiverFullDetailsDto>();

            foreach (var item in response.Items)
            {
                var dto = new KpoReceiverFullDetailsDto
                {
                    // =====================================================
                    // IDENTYFIKACJA
                    // =====================================================
                    CardId = item.KpoId,
                    CardNumber = item.CardNumber,
                    KpoCardNumber = item.KpoCardNumber,
                    KpokCardNumber = item.KpokCardNumber,

                    // =====================================================
                    // STATUS
                    // =====================================================
                    Status = item.Status,
                    CardStatus = item.CardStatus,
                    CardStatusCodeName = item.CardStatusCodeName,
                    IsWithdrawn = item.IsWithdrawn,

                    // =====================================================
                    // ODPAD
                    // =====================================================
                    WasteCodeId = item.WasteCodeId,
                    WasteCode = item.WasteCode,
                    WasteCodeDescription = item.WasteCodeDescription,
                    WasteCodeAndDescription = item.WasteCodeAndDescription,

                    // =====================================================
                    // MASY LISTOWE (SEARCH)
                    // =====================================================
                    DeclaredWasteMass = item.Quantity.HasValue
                        ? (double?)item.Quantity.Value
                        : null,


                    // =====================================================
                    // PODMIOTY – ID
                    // =====================================================
                    SenderCompanyId = item.SenderCompanyId,
                    ReceiverCompanyId = item.ReceiverCompanyId,
                    CarrierCompanyId = item.CarrierCompanyId,

                    // =====================================================
                    // PODMIOTY – DANE PODSTAWOWE (SEARCH)
                    // =====================================================
                    SenderCompanyName = item.SenderCompanyName,
                    ReceiverCompanyName = item.ReceiverNameOrFirstNameAndLastName,
                    ReceiverNip = item.ReceiverNip,
                    ReceiverNipEu = item.ReceiverNipEu,

                    CarrierCompanyName = item.CarrierCompanyName,
                    CarrierNip = item.CarrierNip,
                    CarrierNipEu = item.CarrierNipEu,

                    // =====================================================
                    // TRANSPORT
                    // =====================================================
                    VehicleRegNumber = item.VehicleRegNumber,
                    PlannedTransportTime = item.PlannedTransportTime,
                    RealTransportTime = item.RealTransportTime,

                    // =====================================================
                    // DATY / UŻYTKOWNICY (SEARCH)
                    // =====================================================
                    //CreatedDate = item.CreatedDate,
                    //AcceptanceDate = item.AcceptanceDate,
                    //ReceiveDate = item.ReceiveDate,

                    //CardApprovalTime = item.CardApprovalTime,
                    //CardWithdrawalTime = item.CardWithdrawalTime,

                    GeneratedByUser = item.CreatedByUser,
                    ApprovedByUser = item.ApprovalUser,
                    WithdrawnByUser = item.WithdrawnByUser,

                    // =====================================================
                    // DODATKOWE
                    // =====================================================
                    //InstallationName = item.InstallationName,
                    //WasteProcessId = item.WasteProcessId,
                    Remarks = item.Remarks,
                    AdditionalInfo = item.AdditionalInfo,

                    // =====================================================
                    // KONTEKST
                    // =====================================================
                    CompanyType = (int)companyType

                };


                DebugLogger.Add(
    $"[SEARCH DTO] {dto.CardNumber} | " +
    $"Qty={dto.DeclaredWasteMass} | " +
    $"Sender={dto.SenderCompanyId} | " +
    $"Receiver={dto.ReceiverCompanyId} | " +
    $"Carrier={dto.CarrierCompanyId}"
);



                result.Add(dto);
            }

            return result;
        }

        private async void dgvKpo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var selected = dgvKpo.Rows[e.RowIndex].DataBoundItem as KpoSearchItemDto;
            if (selected == null) return;

            // TESTUJEMY TYLKO RECEIVER (skoro to karta z Twojej listy) ALE Z PEŁNYM LOGOWANIEM
            string kpoId = selected.KpoId.ToString();
            string fullUrl = $"https://api.bdo.mos.gov.pl/api/WasteRegister/WasteTransferCard/v1/Kpo/receiver/details?kpoId={kpoId}";

            DebugLogger.Add($"[DEBUG URL] {fullUrl}");

            // Tworzymy czysty request, żeby mieć pewność co do nagłówków
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _eupToken);

            var response = await _apiClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string rawJson = await response.Content.ReadAsStringAsync();
                DebugLogger.Add("[SUCCESS JSON DETAILS]");
                DebugLogger.Add(rawJson);
            }
            else
            {
                // Tu sprawdzamy co DOKŁADNIE mówi serwer
                string errorBody = await response.Content.ReadAsStringAsync();
                DebugLogger.Add($"[FAIL] Status: {response.StatusCode} | Body: {errorBody}");
            }
        }
    }
}
