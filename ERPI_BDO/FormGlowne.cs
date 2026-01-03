using ERPI_BDO.Api;
using ERPI_BDO.OpenApi.WasteRegister; // Tu nadal trzymamy DTO z NSwag dla listy, np. CompanyEupDto, KpoSearchResult
using ERPI_BDO.OpenApi.WasteRegister.Models; // <- DODANE: tu jest KpoDetailsDto
using ERPI_BDO.Portal;
using Microsoft.Web.WebView2.Core;
using System; // dla DateTime itp.
using System.Collections.Generic; // Dodaj to
using System.Globalization; // dla parsowania daty/czasu
using System.Linq; // Dodaj to
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

                    ReceiveConfirmationDateRange = true,
                    ReceiveConfirmationDateFrom = dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    ReceiveConfirmationDateTo = dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z"),

                    // 🔴 MUSI BYĆ JAWNIE
                    TransportDateRange = false,
                    TransportDateFrom = (string?)null,
                    TransportDateTo = (string?)null
                };


                var receiverList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/receiver/search",
                    receiverCriteria,
                    companyType: 2
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

                    TransportDateRange = true,
                    TransportDateFrom = dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    TransportDateTo = dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z"),

                    // 🔴 MUSI BYĆ JAWNIE
                    ReceiveConfirmationDateRange = false,
                    ReceiveConfirmationDateFrom = (string?)null,
                    ReceiveConfirmationDateTo = (string?)null
                };


                var senderList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/sender/search",
                    senderCriteria,
                    companyType: 0
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

                    TransportDateRange = true,
                    TransportDateFrom = dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    TransportDateTo = dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z"),

                    // 🔴 MUSI BYĆ JAWNIE
                    ReceiveConfirmationDateRange = false,
                    ReceiveConfirmationDateFrom = (string?)null,
                    ReceiveConfirmationDateTo = (string?)null
                };


                var transportList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/carrier/search",
                    transportCriteria,
                    companyType: 1
                );

                // =========================================================
                // BIND GRIDÓW (jak było)
                // =========================================================
                this.Invoke((MethodInvoker)delegate
                {
                    dgvKpo.DataSource = receiverList;
                    dgvKpoSender.DataSource = senderList;
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
        private async Task<List<KpoReceiverFullDetailsDto>>
     PostSearchAndBuildFullDetailsListAsync(
         string searchUrl,
         object searchCriteria,
         int companyType // 0=Sender, 1=Carrier, 2=Receiver
     )
        {
            DebugLogger.Add("=== PostSearchAndBuildFullDetailsListAsync START ===");
            DebugLogger.Add($"SEARCH URL = {searchUrl}");
            DebugLogger.Add($"COMPANY TYPE = {companyType}");

            var result = new List<KpoReceiverFullDetailsDto>();

            // =========================================================
            // HTTP CLIENT (EUP)
            // =========================================================
            using var eupClient = new HttpClient
            {
                BaseAddress = new Uri(BdoConfig.BaseUrl)
            };
            eupClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _eupToken);

            // =========================================================
            // SEARCH
            // =========================================================
            var payloadJson = JsonSerializer.Serialize(searchCriteria);
            DebugLogger.Add($"SEARCH PAYLOAD = {payloadJson}");

            var response = await eupClient.PostAsync(
                searchUrl,
                new StringContent(payloadJson, Encoding.UTF8, "application/json")
            );

            var body = await response.Content.ReadAsStringAsync();

            DebugLogger.Add($"SEARCH STATUS = {(int)response.StatusCode}");
            DebugLogger.Add($"SEARCH BODY LENGTH = {body.Length}");

            if (!response.IsSuccessStatusCode)
            {
                DebugLogger.Add("SEARCH ERROR BODY:");
                DebugLogger.Add(body);
                return result;
            }

            using var doc = JsonDocument.Parse(body);

            if (!doc.RootElement.TryGetProperty("items", out var items) ||
                items.ValueKind != JsonValueKind.Array)
            {
                DebugLogger.Add("SEARCH: items[] not found");
                return result;
            }

            DebugLogger.Add($"SEARCH ITEMS COUNT = {items.GetArrayLength()}");

            // =========================================================
            // ITERACJA PO KARTACH
            // =========================================================
            foreach (var item in items.EnumerateArray())
            {
                var kpoId = GetGuid(item, "kpoId");
                if (kpoId == null)
                {
                    DebugLogger.Add("SKIP ITEM: kpoId is null");
                    continue;
                }

                DebugLogger.Add($"--- KPO {kpoId} START ---");

                var dto = new KpoReceiverFullDetailsDto
                {
                    // =================================================
                    // IDENTYFIKACJA
                    // =================================================
                    KpoId = kpoId.Value,
                    Year = GetInt(item, "year"),
                    CardNumber = GetString(item, "cardNumber"),

                    // =================================================
                    // STATUS (LIST)
                    // =================================================
                    List_CardStatus = GetString(item, "cardStatus"),
                    List_CardStatusId = GetInt(item, "cardStatusId"),
                    List_CardStatusCodeName = GetString(item, "cardStatusCodeName"),

                    // =================================================
                    // ODPAD (LIST)
                    // =================================================
                    WasteCodeId = GetInt(item, "wasteCodeId"),
                    WasteCode = GetString(item, "wasteCode"),
                    WasteCodeDescription = GetString(item, "wasteCodeDescription"),
                    WasteCodeExtended = GetBool(item, "wasteCodeExtended"),

                    // =================================================
                    // DATY / MASY (LIST)
                    // =================================================
                    List_PlannedTransportTime = GetDate(item, "plannedTransportTime"),
                    List_RealTransportTime = GetDate(item, "realTransportTime"),
                    List_ReceiveConfirmationTime = GetDate(item, "receiveConfirmationTime"),
                    List_WasteMass = GetDecimal(item, "wasteMass"),

                    // =================================================
                    // SENDER (LIST)
                    // =================================================
                    Sender_List_CompanyId = GetGuid(item, "senderCompanyId"),
                    Sender_List_EupId = GetGuid(item, "senderEupId"),
                    Sender_List_CompanyName = GetString(item, "senderCompanyName"),
                    Sender_List_FirstNameAndLastName =
                        GetString(item, "senderFirstNameAndLastName"),

                    // =================================================
                    // RECEIVER (LIST)
                    // =================================================
                    Receiver_List_CompanyId = GetGuid(item, "receiverCompanyId"),
                    Receiver_List_EupId = GetGuid(item, "receiverEupId"),
                    Receiver_List_CompanyName = GetString(item, "receiverCompanyName"),
                    Receiver_List_FirstNameAndLastName =
                        GetString(item, "receiverFirstNameAndLastName"),

                    // =================================================
                    // CARRIER (LIST)
                    // =================================================
                    Carrier_List_CompanyId = GetGuid(item, "carrierCompanyId"),
                    Carrier_List_EupId = GetGuid(item, "carrierEupId"),
                    Carrier_List_CompanyName = GetString(item, "carrierCompanyName"),
                    Carrier_List_VehicleRegNumber =
                        GetString(item, "vehicleRegNumber")
                };

                // =====================================================
                // DETAILS (TYLKO TEN companyType)
                // =====================================================
                var details = await _bdoService.GetKpoDetailsAsync(
                    kpoId.Value,
                    eupClient,
                    companyType,
                    DebugLogger.Add
                );

                if (details != null)
                {
                    DebugLogger.Add($"DETAILS OK for {kpoId}");

                    // =================================================
                    // SENDER DETAILS
                    // =================================================
                    dto.Sender_Details_CompanyId = details.SenderCompanyId;
                    dto.Sender_Details_EupId = details.SenderEupId;
                    dto.Sender_Details_CompanyName = details.SenderCompanyName;
                    dto.Sender_Details_FirstNameAndLastName =
                        details.SenderFirstNameAndLastName;
                    dto.Sender_Details_IdentificationNumber =
                        details.SenderIdentificationNumber;
                    dto.Sender_Details_Nip = details.SenderNip;

                    // =================================================
                    // RECEIVER DETAILS
                    // =================================================
                    dto.Receiver_Details_CompanyId = details.ReceiverCompanyId;
                    dto.Receiver_Details_EupId = details.ReceiverEupId;
                    dto.Receiver_Details_CompanyName = details.ReceiverCompanyName;
                    dto.Receiver_Details_FirstNameAndLastName =
                        details.ReceiverFirstNameAndLastName;
                    dto.Receiver_Details_IdentificationNumber =
                        details.ReceiverIdentificationNumber;
                    dto.Receiver_Details_Nip = details.ReceiverNip;

                    // =================================================
                    // CARRIER DETAILS
                    // =================================================
                    dto.Carrier_Details_CompanyId = details.CarrierCompanyId;
                    dto.Carrier_Details_EupId = details.CarrierEupId;
                    dto.Carrier_Details_CompanyName = details.CarrierCompanyName;
                    dto.Carrier_Details_IdentificationNumber =
                        details.IdentificationNumber;
                    dto.Carrier_Details_Nip = details.Nip;
                    dto.Carrier_Details_EuNip = details.EuNip;
                    dto.Carrier_Details_RegistryNumber =
                        details.RegistrationNumber;

                    // =================================================
                    // ADRES (DETAILS – wspólny)
                    // =================================================

                }
                else
                {
                    DebugLogger.Add($"DETAILS NOT FOUND for {kpoId}");
                }

                result.Add(dto);
                DebugLogger.Add($"--- KPO {kpoId} END ---");
            }

            DebugLogger.Add(
                $"=== PostSearchAndBuildFullDetailsListAsync END | COUNT={result.Count} ===");

            return result;
        }


        private async Task<List<KpoReceiverFullDetailsDto>> PostSearchAndBuildFullDetailsListAsync1(
      string relativeUrl,
      object searchCriteria)
        {
            DebugLogger.Add("=== PostSearchAndBuildFullDetailsListAsync START (3×DETAILS) ===");

            var result = new List<KpoReceiverFullDetailsDto>();

            if (_apiClient == null || _eupContext == null)
            {
                DebugLogger.Add("ERROR: _apiClient or _eupContext is NULL");
                return result;
            }

            var payload = JsonSerializer.Serialize(searchCriteria);
            DebugLogger.Add($"SEARCH URL = {relativeUrl}");
            DebugLogger.Add($"SEARCH PAYLOAD = {payload}");

            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Eup-Id", _eupContext.EupId.ToString());
            request.Headers.Add("X-Company-Id", _eupContext.CompanyId.ToString());
            request.Headers.Add("X-Year-Context", _eupContext.Year.ToString());

            HttpResponseMessage response;
            try
            {
                response = await _apiClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                DebugLogger.Add($"HTTP EXCEPTION (SEARCH): {ex}");
                return result;
            }

            var body = await response.Content.ReadAsStringAsync();
            DebugLogger.Add($"SEARCH STATUS = {(int)response.StatusCode}");
            DebugLogger.Add($"SEARCH BODY LENGTH = {body.Length}");

            if (!response.IsSuccessStatusCode)
            {
                DebugLogger.Add("SEARCH ERROR BODY:");
                DebugLogger.Add(body);
                return result;
            }

            using var doc = JsonDocument.Parse(body);

            if (!doc.RootElement.TryGetProperty("items", out var items) ||
                items.ValueKind != JsonValueKind.Array)
            {
                DebugLogger.Add("ERROR: SEARCH response has no items[]");
                return result;
            }

            DebugLogger.Add($"SEARCH ITEMS COUNT = {items.GetArrayLength()}");

            // =========================================================
            // PĘTLA PO KPO
            // =========================================================

            foreach (var item in items.EnumerateArray())
            {
                if (!item.TryGetProperty("kpoId", out var kpoIdProp) ||
                    !Guid.TryParse(kpoIdProp.GetString(), out var kpoId))
                {
                    DebugLogger.Add("SKIP: item without valid kpoId");
                    continue;
                }

                DebugLogger.Add($"--- KPO {kpoId} START ---");

                var dto = new KpoReceiverFullDetailsDto
                {
                    KpoId = kpoId,
                    Year = item.TryGetProperty("year", out var y) ? y.GetInt32() : null,
                    CardNumber = item.TryGetProperty("cardNumber", out var cn) ? cn.GetString() : null,

                    // ================= LIST – SENDER =================
                    Sender_List_CompanyId = GetGuid(item, "senderCompanyId"),
                    Sender_List_EupId = GetGuid(item, "senderEupId"),
                    Sender_List_CompanyName = GetString(item, "senderCompanyName", "senderName"),
                    Sender_List_FirstNameAndLastName = GetString(item, "senderFirstNameAndLastName"),

                    // ================= LIST – CARRIER =================
                    Carrier_List_CompanyId = GetGuid(item, "carrierCompanyId"),
                    Carrier_List_EupId = GetGuid(item, "carrierEupId"),
                    Carrier_List_CompanyName = GetString(item, "carrierCompanyName"),
                    Carrier_List_VehicleRegNumber = GetString(item, "vehicleRegNumber"),

                    // ================= LIST – RECEIVER ================
                    Receiver_List_CompanyId = GetGuid(item, "receiverCompanyId"),
                    Receiver_List_EupId = GetGuid(item, "receiverEupId"),
                    Receiver_List_CompanyName = GetString(item, "receiverCompanyName", "receiverName"),
                    Receiver_List_FirstNameAndLastName = GetString(item, "receiverFirstNameAndLastName"),

                    // ================= STATUS / ODPAD =================
                    List_CardStatus = GetString(item, "cardStatus"),
                    List_CardStatusId = GetInt(item, "cardStatusId"),
                    List_CardStatusCodeName = GetString(item, "cardStatusCodeName"),

                    WasteCodeId = GetInt(item, "wasteCodeId"),
                    WasteCode = GetString(item, "wasteCode"),
                    WasteCodeDescription = GetString(item, "wasteCodeDescription"),
                    WasteCodeExtended = GetBool(item, "wasteCodeExtended"),

                    List_WasteMass = GetDecimal(item, "wasteMass", "quantity"),
                    List_PlannedTransportTime = GetDate(item, "plannedTransportTime"),
                    List_RealTransportTime = GetDate(item, "realTransportTime"),
                    List_ReceiveConfirmationTime = GetDate(item, "receiveConfirmationTime")
                };

                // =====================================================
                // DETAILS – SENDER (CompanyType = 0)
                // =====================================================
                DebugLogger.Add($"[DETAILS:SENDER] Fetching for KPO {kpoId}");
                var senderDetails = await _bdoService.GetKpoDetailsAsync(
                    kpoId, _apiClient, 0, DebugLogger.Add);

                if (senderDetails != null)
                {
                    dto.Sender_Details_CompanyId = senderDetails.SenderCompanyId;
                    dto.Sender_Details_EupId = senderDetails.SenderEupId;
                    dto.Sender_Details_CompanyName = senderDetails.SenderCompanyName;
                    dto.Sender_Details_FirstNameAndLastName = senderDetails.SenderFirstNameAndLastName;
                    dto.Sender_Details_IdentificationNumber = senderDetails.SenderIdentificationNumber;
                    dto.Sender_Details_Nip = senderDetails.SenderNip;

                    dto.Sender_Details_AddressHtml = senderDetails.AddressHtml;
                    dto.Sender_Details_TerytPk = senderDetails.TerytPk;
                    dto.Sender_Details_PostalCode = senderDetails.PostalCode;
                    dto.Sender_Details_Locality = senderDetails.Locality;
                    dto.Sender_Details_Street = senderDetails.Street;
                    dto.Sender_Details_BuildingNumber = senderDetails.BuildingNumber;
                    dto.Sender_Details_LocalNumber = senderDetails.LocalNumber;
                    dto.Sender_Details_CountryName = senderDetails.CountryName;
                }

                // =====================================================
                // DETAILS – CARRIER (CompanyType = 1)
                // =====================================================
                DebugLogger.Add($"[DETAILS:CARRIER] Fetching for KPO {kpoId}");
                var carrierDetails = await _bdoService.GetKpoDetailsAsync(
                    kpoId, _apiClient, 1, DebugLogger.Add);

                if (carrierDetails != null)
                {
                    dto.Carrier_Details_CompanyId = carrierDetails.CarrierCompanyId;
                    dto.Carrier_Details_EupId = carrierDetails.CarrierEupId;
                    dto.Carrier_Details_CompanyName = carrierDetails.CarrierCompanyName;
                    dto.Carrier_Details_IdentificationNumber = carrierDetails.IdentificationNumber;
                    dto.Carrier_Details_Nip = carrierDetails.Nip;
                    dto.Carrier_Details_EuNip = carrierDetails.EuNip;
                    dto.Carrier_Details_RegistryNumber = carrierDetails.RegistrationNumber;

                    dto.Carrier_Details_AddressHtml = carrierDetails.AddressHtml;
                    dto.Carrier_Details_TerytPk = carrierDetails.TerytPk;
                    dto.Carrier_Details_PostalCode = carrierDetails.PostalCode;
                    dto.Carrier_Details_Locality = carrierDetails.Locality;
                    dto.Carrier_Details_Street = carrierDetails.Street;
                    dto.Carrier_Details_BuildingNumber = carrierDetails.BuildingNumber;
                    dto.Carrier_Details_LocalNumber = carrierDetails.LocalNumber;
                    dto.Carrier_Details_CountryName = carrierDetails.CountryName;
                }

                // =====================================================
                // DETAILS – RECEIVER (CompanyType = 2)
                // =====================================================
                DebugLogger.Add($"[DETAILS:RECEIVER] Fetching for KPO {kpoId}");
                var receiverDetails = await _bdoService.GetKpoDetailsAsync(
                    kpoId, _apiClient, 2, DebugLogger.Add);

                if (receiverDetails != null)
                {
                    dto.Receiver_Details_CompanyId = receiverDetails.ReceiverCompanyId;
                    dto.Receiver_Details_EupId = receiverDetails.ReceiverEupId;
                    dto.Receiver_Details_CompanyName = receiverDetails.ReceiverCompanyName;
                    dto.Receiver_Details_FirstNameAndLastName = receiverDetails.ReceiverFirstNameAndLastName;
                    dto.Receiver_Details_IdentificationNumber = receiverDetails.ReceiverIdentificationNumber;
                    dto.Receiver_Details_Nip = receiverDetails.ReceiverNip;

                    dto.Receiver_Details_AddressHtml = receiverDetails.AddressHtml;
                    dto.Receiver_Details_TerytPk = receiverDetails.TerytPk;
                    dto.Receiver_Details_PostalCode = receiverDetails.PostalCode;
                    dto.Receiver_Details_Locality = receiverDetails.Locality;
                    dto.Receiver_Details_Street = receiverDetails.Street;
                    dto.Receiver_Details_BuildingNumber = receiverDetails.BuildingNumber;
                    dto.Receiver_Details_LocalNumber = receiverDetails.LocalNumber;
                    dto.Receiver_Details_CountryName = receiverDetails.CountryName;
                }

                // =====================================================
                // EFFECTIVE
                // =====================================================
                dto.Effective_WasteMass =
                    receiverDetails?.CorrectedWasteMass
                    ?? receiverDetails?.RevisedWasteMass
                    ?? receiverDetails?.WasteMass
                    ?? dto.List_WasteMass;

                dto.Effective_TransportTime =
                    receiverDetails?.RealTransportTime
                    ?? receiverDetails?.PlannedTransportTime;

                dto.Effective_IsCorrected =
                    receiverDetails?.CorrectedWasteMass.HasValue == true
                    || receiverDetails?.IsRevised == true;

                result.Add(dto);

                DebugLogger.Add($"--- KPO {kpoId} END ---");
            }

            DebugLogger.Add($"=== PostSearchAndBuildFullDetailsListAsync END | COUNT={result.Count} ===");
            return result;
        }




       

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
    }
}
