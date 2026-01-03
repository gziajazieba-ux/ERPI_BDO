using ERPI_BDO.Api;
using ERPI_BDO.OpenApi.WasteRegister; // Tu nadal trzymamy DTO z NSwag dla listy, np. CompanyEupDto, KpoSearchResult
using ERPI_BDO.OpenApi.WasteRegister.Models; // <- DODANE: tu jest KpoDetailsDto
using ERPI_BDO.Portal;
using Microsoft.Web.WebView2.Core;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Collections.Generic; // Dodaj to
using System.Linq; // Dodaj to
using System; // dla DateTime itp.
using System.Globalization; // dla parsowania daty/czasu
using System.Windows.Forms; // używane typy WinForms

namespace ERPI_BDO
{//komentarz
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
            DebugLogger.Add("=== btnPobierzKpo_Click (Unified for receiver/sender/transport) ===");

            StartKpoLoading("Czytam karty");

            try
            {
                if (_eupContext == null || string.IsNullOrEmpty(_eupToken))
                {
                    DebugLogger.Add("Blad: Brak kontekstu lub tokena EUP.");
                    lblStatus.Text = "Błąd: Brakuje kontekstu EUP. Wybierz EUP ponownie.";
                    lblStatus.ForeColor = Color.Red;
                    StopKpoLoading();
                    return;
                }

                if (_apiClient == null)
                {
                    DebugLogger.Add("Blad: Klient API nie jest zainicjalizowany. Wybierz EUP ponownie.");
                    lblStatus.Text = "Błąd: Klient API nie jest gotowy. Wybierz EUP ponownie.";
                    lblStatus.ForeColor = Color.Red;
                    StopKpoLoading();
                    return;
                }

                // 1) Receiver (przejmujący) - używamy ReceiveConfirmationDateRange
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
                    TransportDateRange = false,
                    TransportDateFrom = (string?)null,
                    TransportDateTo = (string?)null
                };

                var receiverList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/receiver/search",
                    receiverCriteria,
                    companyType: 2 // ReceiverCompany
                );

                // 2) Sender (przekazujący) - używamy TransportDateRange (zgodnie z Twoim wcześniejszym kodem)
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
                    ReceiveConfirmationDateRange = false,
                    ReceiveConfirmationDateFrom = (string?)null,
                    ReceiveConfirmationDateTo = (string?)null,
                    TransportDateRange = true,
                    TransportDateFrom = dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    TransportDateTo = dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z")
                };

                var senderList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/sender/search",
                    senderCriteria,
                    companyType: 0 // SenderCompany
                );

                // 3) Transport / Carrier — endpointy w BDO mogą nazywać się "carrier/search" lub "transport/search".
                // Jeśli w Twojej dokumentacji inna nazwa — zmień poniższy URL odpowiednio.
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
                    ReceiveConfirmationDateRange = false,
                    TransportDateRange = true,
                    TransportDateFrom = dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    TransportDateTo = dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z")
                };

                // Uwaga: jeśli Twój API wymaga innej ścieżki dla transportującego, zamień "/Kpo/carrier/search" na właściwą.
                var transportList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/carrier/search",
                    transportCriteria,
                    companyType: 1 // CarrierCompany
                );

                // Bind i konfiguracja kolumn - użyjemy tej samej konfiguracji kolumn dla wszystkich trzech gridów
                this.Invoke((MethodInvoker)delegate
                {
                    // Receiver grid
                    dgvKpo.SuspendLayout();
                    dgvKpo.DataSource = null;
                    ConfigureDgvKpoColumns(dgvKpo);
                    dgvKpo.DataSource = receiverList;
                    dgvKpo.ResumeLayout();

                    // Sender grid
                    dgvKpoSender.SuspendLayout();
                    dgvKpoSender.DataSource = null;
                    ConfigureDgvKpoColumns(dgvKpoSender);
                    dgvKpoSender.DataSource = senderList;
                    dgvKpoSender.ResumeLayout();

                    // Transport grid
                    dgvKpoTransport.SuspendLayout();
                    dgvKpoTransport.DataSource = null;
                    ConfigureDgvKpoColumns(dgvKpoTransport);
                    dgvKpoTransport.DataSource = transportList;
                    dgvKpoTransport.ResumeLayout();

                    lblStatus.Text = $"Pobrano: receiver={receiverList.Count}, sender={senderList.Count}, transport={transportList.Count}";
                    lblStatus.ForeColor = Color.Green;
                });
            }
            catch (Exception ex)
            {
                DebugLogger.Add($"Wyjatek: {ex.Message}");
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = "Wystąpił wyjątek podczas pobierania KPO.";
                    lblStatus.ForeColor = Color.Red;
                    MessageBox.Show($"Wystąpił wyjątek podczas pobierania KPO:\n{ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
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

        private async Task<List<KpoReceiverFullDetailsDto>> PostSearchAndBuildFullDetailsListAsync(string relativeUrl, object searchCriteria, int companyType)
        {
            var list = new List<KpoReceiverFullDetailsDto>();

            string jsonPayload = JsonSerializer.Serialize(searchCriteria);
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl)
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Eup-Id", _eupContext!.EupId.ToString());
            request.Headers.Add("X-Company-Id", _eupContext.CompanyId.ToString());
            request.Headers.Add("X-Year-Context", searchCriteria.GetType().GetProperty("Year")?.GetValue(searchCriteria)?.ToString() ?? DateTime.Now.Year.ToString());

            var response = await _apiClient!.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            DebugLogger.Add($"Status HTTP: {(int)response.StatusCode} dla {relativeUrl}");
            DebugLogger.Add($"Odpowiedź API: {body}");

            if (!response.IsSuccessStatusCode)
            {
                DebugLogger.Add($"Blad BDO przy {relativeUrl}: {body}");
                return list;
            }

            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
                return list;

            string? GetString(JsonElement el, params string[] names)
            {
                foreach (var n in names)
                    if (el.TryGetProperty(n, out var p) && p.ValueKind != JsonValueKind.Null)
                        return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
                return null;
            }

            decimal? GetDecimal(JsonElement el, params string[] names)
            {
                foreach (var n in names)
                    if (el.TryGetProperty(n, out var p) && (p.ValueKind == JsonValueKind.Number || p.ValueKind == JsonValueKind.String))
                    {
                        if (p.TryGetDecimal(out var d)) return d;
                        if (decimal.TryParse(p.GetRawText().Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
                    }
                return null;
            }

            DateTime? GetDateTime(JsonElement el, params string[] names)
            {
                foreach (var n in names)
                    if (el.TryGetProperty(n, out var p) && p.ValueKind == JsonValueKind.String)
                    {
                        if (DateTime.TryParse(p.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                            return dt;
                    }
                return null;
            }

            Guid? GetGuid(JsonElement el, params string[] names)
            {
                foreach (var n in names)
                    if (el.TryGetProperty(n, out var p) && p.ValueKind == JsonValueKind.String)
                    {
                        if (Guid.TryParse(p.GetString(), out var g)) return g;
                    }
                return null;
            }

            DateTime? ParseDateAndTimeStrings(string? dateStr, string? timeStr)
            {
                if (string.IsNullOrWhiteSpace(dateStr) && string.IsNullOrWhiteSpace(timeStr))
                    return null;

                if (!string.IsNullOrWhiteSpace(dateStr))
                {
                    if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dtFull))
                        return dtFull;
                }

                if (!string.IsNullOrWhiteSpace(dateStr) && !string.IsNullOrWhiteSpace(timeStr))
                {
                    var combined = $"{dateStr} {timeStr}";
                    if (DateTime.TryParse(combined, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                        return dt;
                }

                if (string.IsNullOrWhiteSpace(dateStr) && !string.IsNullOrWhiteSpace(timeStr))
                {
                    if (TimeSpan.TryParse(timeStr, CultureInfo.InvariantCulture, out var ts))
                        return DateTime.Today.Add(ts);
                }

                return null;
            }

            foreach (var itemEl in items.EnumerateArray())
            {
                try
                {
                    var kpoId = GetGuid(itemEl, "kpoId", "KpoId");
                    if (kpoId == null) { DebugLogger.Add("Brak kpoId w elemencie listy — pomijam."); continue; }

                    var cardNumber = GetString(itemEl, "cardNumber", "kpoNumber", "KpoNumber");
                    var listWasteMass = GetDecimal(itemEl, "wasteMass", "quantity", "Quantity");

                    KpoDetailsDto? details = null;
                    try
                    {
                        details = await _bdoService.GetKpoDetailsAsync(kpoId.Value, _apiClient, companyType, DebugLogger.Add);
                    }
                    catch (Exception ex)
                    {
                        DebugLogger.Add($"Błąd pobierania szczegółów dla {kpoId}: {ex.Message}");
                    }

                    decimal? wasteMass = details?.WasteMass ?? listWasteMass;
                    decimal? corrected = GetDecimal(itemEl, "correctedWasteMass");
                    decimal? revised = GetDecimal(itemEl, "revisedWasteMass");

                    DateTime? plannedTransport = details?.PlannedTransportTime;
                    DateTime? realTransport = null;
                    if (details != null)
                    {
                        realTransport = ParseDateAndTimeStrings(details.RealTransportDate, details.RealTransportTime) ?? GetDateTime(itemEl, "realTransportTime");
                    }
                    else
                    {
                        realTransport = GetDateTime(itemEl, "realTransportTime");
                    }

                    DateTime? cardApprovalTime = ParseDateAndTimeStrings(details?.ApprovalDate, details?.ApprovalTime) ?? GetDateTime(itemEl, "cardApprovalTime");
                    DateTime? receiveConfirmationTime = ParseDateAndTimeStrings(details?.ReceiveConfirmationDate, details?.ReceiveConfirmationTime) ?? GetDateTime(itemEl, "acceptanceDate", "receiveConfirmationTime");

                    var dto = new KpoReceiverFullDetailsDto
                    {
                        KpoId = kpoId.Value,
                        Year = details?.Year,
                        CardNumber = details?.CardNumber ?? cardNumber,
                        CardStatus = details?.CardStatus ?? GetString(itemEl, "cardStatus"),
                        CardStatusId = details?.CardStatusId,
                        CardStatusCodeName = details?.CardStatusCodeName ?? GetString(itemEl, "cardStatusCodeName"),

                        SenderCompanyId = details?.SenderCompanyId ?? GetGuid(itemEl, "senderCompanyId"),
                        SenderEupId = details?.SenderEupId ?? GetGuid(itemEl, "senderEupId"),
                        SenderCompanyName = details?.SenderCompanyName ?? details?.SenderName ?? GetString(itemEl, "senderCompanyName", "senderName"),
                        SenderFirstNameAndLastName = details?.SenderFirstNameAndLastName ?? GetString(itemEl, "senderFirstNameAndLastName"),
                        SenderName = details?.SenderName ?? GetString(itemEl, "senderName"),

                        ReceiverCompanyId = details?.ReceiverCompanyId ?? GetGuid(itemEl, "receiverCompanyId"),
                        ReceiverEupId = details?.ReceiverEupId ?? GetGuid(itemEl, "receiverEupId"),
                        // POPRAWKA: używamy właściwej nazwy pola z KpoDetailsDto (ReceiverFirstNameAndLastName)
                        ReceiverFirstAndLastName = details?.ReceiverFirstNameAndLastName 
                            ?? GetString(itemEl, "receiverFirstNameAndLastName", "receiverFirstAndLastName", "receiverFirstAndLast"),
                        ReceiverCompanyName = details?.ReceiverCompanyName ?? GetString(itemEl, "receiverCompanyName", "receiverName"),
                        ReceiverName = details?.ReceiverName ?? GetString(itemEl, "receiverName"),

                        CarrierCompanyId = details?.CarrierCompanyId ?? GetGuid(itemEl, "carrierCompanyId"),
                        CarrierEupId = null,
                        CarrierCompanyName = details?.CarrierCompanyName ?? details?.CarrierName ?? GetString(itemEl, "carrierCompanyName", "carrierName"),

                        WasteCode = details?.WasteCode ?? GetString(itemEl, "wasteCode"),
                        WasteCodeId = details?.WasteCodeId,
                        WasteCodeDescription = details?.WasteCodeAndDescription ?? GetString(itemEl, "wasteCodeDescription", "wasteCodeAndDescription"),
                        WasteCodeExtended = details?.WasteCodeExtended,
                        WasteCodeExtendedDescription = details?.WasteCodeExtendedDescription,
                        WasteMass = wasteMass,
                        CorrectedWasteMass = corrected,
                        RevisedWasteMass = revised,
                        RevisedWasteCodeId = null,
                        IsWasteGenerating = details?.IsWasteGenerating,
                        WasteGeneratingAdditionalInfo = details?.WasteGeneratingAdditionalInfo,
                        WasteGeneratedTerytPk = details?.WasteGeneratedTerytPk,
                        WasteProcessId = details?.WasteProcessId,

                        IsRevised = details?.IsRevised ?? (GetString(itemEl, "isRevised") == "true"),
                        RevisedAt = GetDateTime(itemEl, "revisedAt"),
                        RevisedBy = GetString(itemEl, "revisedBy") ?? details?.ApprovedByUser,

                        PlannedTransportTime = plannedTransport,
                        RealTransportTime = realTransport,
                        ReceiveConfirmationTime = receiveConfirmationTime,
                        TransportConfirmationTime = GetDateTime(itemEl, "transportConfirmationTime"),
                        CardApprovalTime = cardApprovalTime,
                        CardRejectionTime = GetDateTime(itemEl, "cardRejectionTime"),
                        GeneratingConfirmationTime = GetDateTime(itemEl, "generatingConfirmationTime"),
                        ApprovalUser = details?.ApprovedByUser ?? GetString(itemEl, "approvalUser"),
                        TransportConfirmationUser = GetString(itemEl, "transportConfirmationUser"),
                        GeneratingConfirmationUser = GetString(itemEl, "generatingConfirmationUser"),
                        ReceiveConfirmationUser = details?.ReceiveConfirmedByUser ?? GetString(itemEl, "receiveConfirmationUser"),
                        RejectedByUser = details?.RejectedByUser ?? GetString(itemEl, "rejectedByUser", "rejectedByUserFirstNameAndLastName"),

                        VehicleRegNumber = details?.VehicleRegNumber ?? GetString(itemEl, "vehicleRegNumber", "vehicleRegNo"),
                        CertificateNumberAndBoxNumbers = details?.CertificateNumberAndBoxNumbers ?? GetString(itemEl, "certificateNumberAndBoxNumbers"),
                        AdditionalInfo = details?.AdditionalInfo ?? GetString(itemEl, "additionalInfo"),
                        HazardousWasteReclassification = details?.HazardousWasteReclassification,
                        HazardousWasteReclassificationDescription = details?.HazardousWasteReclassificationDescription ?? GetString(itemEl, "hazardousWasteReclassificationDescription"),
                        Remarks = details?.Remarks ?? GetString(itemEl, "remarks"),

                        CreatedBy = GetString(itemEl, "createdBy"),
                        CreatedDate = GetDateTime(itemEl, "kpoLastModifiedAt") ?? DateTime.MinValue,
                        IdentificationNumber = details?.IdentificationNumber ?? details?.SenderIdentificationNumber ?? GetString(itemEl, "identificationNumber"),
                        Nip = details?.Nip ?? details?.SenderNip ?? GetString(itemEl, "nip"),
                        AddressHtml = details?.AddressHtml ?? details?.SenderAddress ?? details?.SenderEupAddress ?? GetString(itemEl, "addressHtml"),
                        PostalCode = details?.PostalCode ?? details?.SenderEupAddress ?? GetString(itemEl, "postalCode"),
                        CountryName = details?.CountryName ?? GetString(itemEl, "countryName"),
                        Locality = details?.Locality ?? GetString(itemEl, "locality"),
                        Street = details?.Street ?? GetString(itemEl, "street"),
                        BuildingNumber = details?.BuildingNumber ?? GetString(itemEl, "buildingNumber"),
                        LocalNumber = details?.LocalNumber ?? GetString(itemEl, "localNumber"),
                        AdditionalInfoForUi = details?.SenderEupName ?? details?.SenderEupNumber ?? GetString(itemEl, "additionalInfoUi")
                    };

                    dto.Quantity = dto.RevisedWasteMass ?? dto.WasteMass ?? dto.CorrectedWasteMass ?? listWasteMass ?? 0m;

                    DebugLogger.Add($"KPO {dto.CardNumber ?? kpoId.ToString()}: wasteMass={dto.WasteMass?.ToString() ?? "null"}, corrected={dto.CorrectedWasteMass?.ToString() ?? "null"}, revised={dto.RevisedWasteMass?.ToString() ?? "null"}, Quantity={dto.Quantity}");

                    list.Add(dto);
                }
                catch (Exception ex)
                {
                    DebugLogger.Add($"Wyjątek podczas mapowania elementu listy KPO: {ex.Message}");
                    continue;
                }
            }

            return list;
        }
        // --- Początek nowego kodu dla KPO Przekazującego ---
        private async void btnPobierzKpoSender_Click(object sender, EventArgs e)
        {
            DebugLogger.Add("=== btnPobierzKpoSender_Click (shared PostSearchAndBuildFullDetailsListAsync) ===");

            try
            {
                if (_eupContext == null || string.IsNullOrEmpty(_eupToken))
                {
                    DebugLogger.Add("Blad: Brak kontekstu lub tokena EUP.");
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Błąd: Brak wybranego EUP lub tokena.";
                        lblStatus.ForeColor = Color.Red;
                    });
                    return;
                }

                if (_apiClient == null)
                {
                    DebugLogger.Add("Blad: Klient API nie jest zainicjalizowany. Wybierz EUP ponownie.");
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Błąd: Klient API nie jest gotowy.";
                        lblStatus.ForeColor = Color.Red;
                    });
                    return;
                }

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
                    ReceiveConfirmationDateRange = false,
                    ReceiveConfirmationDateFrom = (string?)null,
                    ReceiveConfirmationDateTo = (string?)null,
                    TransportDateRange = true,
                    TransportDateFrom = dtKpoOd.Value.ToString("yyyy-MM-ddT00:00:00.000Z"),
                    TransportDateTo = dtKpoDo.Value.ToString("yyyy-MM-ddT23:59:59.999Z")
                };

                var senderList = await PostSearchAndBuildFullDetailsListAsync(
                    "/api/WasteRegister/WasteTransferCard/v1/Kpo/sender/search",
                    senderCriteria,
                    companyType: 0 // SenderCompany
                );

                this.Invoke((MethodInvoker)delegate
                {
                    dgvKpoSender.SuspendLayout();
                    dgvKpoSender.DataSource = null;
                    ConfigureDgvKpoColumns(dgvKpoSender);
                    dgvKpoSender.DataSource = senderList;
                    dgvKpoSender.ResumeLayout();

                    lblStatus.Text = $"Pobrano {senderList.Count} kart (Sender).";
                    lblStatus.ForeColor = Color.Green;
                });

                DebugLogger.Add("Sukces! Dane KPO Przekazującego pobrane i zmapowane do DTO.");
            }
            catch (Exception ex)
            {
                DebugLogger.Add($"Wyjatek dla KPO Przekazującego: {ex.Message}");
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = "Wystąpił wyjątek podczas pobierania KPO Przekazującego.";
                    lblStatus.ForeColor = Color.Red;
                    MessageBox.Show($"Wystąpił wyjątek podczas pobierania KPO Przekazującego:\n{ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }
        // --- Koniec nowego kodu dla KPO Przekazującego ---


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
