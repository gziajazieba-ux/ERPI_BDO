using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ERPI_BDO.Portal
{
    public class BdoPortalSession
    {
        public HttpClient Client { get; }
        public CookieContainer Cookies { get; }
        public bool IsLoggedIn { get; private set; }

        private readonly Action<string> _debug;

        public BdoPortalSession(Action<string> debug)
        {
            _debug = debug;

            Cookies = new CookieContainer();

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = Cookies,
                AllowAutoRedirect = true,
                AutomaticDecompression =
                    DecompressionMethods.GZip |
                    DecompressionMethods.Deflate
            };

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://rejestr-bdo.mos.gov.pl"),
                Timeout = TimeSpan.FromSeconds(60)
            };

            // UDAJEMY PRAWDZIWĄ PRZEGLĄDARKĘ
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            Client.DefaultRequestHeaders.Accept.ParseAdd(
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        }

        /// <summary>
        /// PRAWDZIWY login portalowy BDO – etap diagnostyczny
        /// </summary>
        public async Task LoginAsync(string login, string password)
        {
            _debug("=== ETAP 2.5: LOGIN PORTALOWY BDO ===");

            // 1️⃣ GET strony logowania
            _debug("GET /Account/Login");

            var loginPageResp = await Client.GetAsync("/Account/Login");
            var loginPageHtml = await loginPageResp.Content.ReadAsStringAsync();

            _debug($"GET Login STATUS = {(int)loginPageResp.StatusCode}");
            _debug($"Login HTML length = {loginPageHtml.Length}");

            DumpCookies("Po GET /Account/Login");

            DumpHtmlSnippet(loginPageHtml, "LOGIN PAGE HTML");

            // 2️⃣ Wyciągamy __RequestVerificationToken
            var token = ExtractAntiForgeryToken(loginPageHtml);

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Nie znaleziono __RequestVerificationToken");

            _debug($"AntiForgeryToken = {token}");

            // 3️⃣ POST formularza logowania (REALNY)
            _debug("POST /Account/Login (realny formularz)");

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("Username", login),
                new KeyValuePair<string,string>("Password", password),
                new KeyValuePair<string,string>("__RequestVerificationToken", token)
            });

            var postResp = await Client.PostAsync("/Account/Login", form);
            var postHtml = await postResp.Content.ReadAsStringAsync();

            _debug($"POST Login STATUS = {(int)postResp.StatusCode}");
            _debug($"POST Login HTML length = {postHtml.Length}");

            DumpCookies("Po POST /Account/Login");
            DumpHtmlSnippet(postHtml, "POST LOGIN RESPONSE");

            // 4️⃣ Heurystyka: sprawdzamy czy nadal jest formularz logowania
            IsLoggedIn = !postHtml.Contains("Zaloguj", StringComparison.OrdinalIgnoreCase)
                         && Cookies.Count > 0;

            _debug($"PORTAL LOGIN RESULT = {IsLoggedIn}");
            _debug("=== KONIEC ETAP 2.5 ===");
        }

        // ----------------- POMOCNICZE -----------------

        private void DumpCookies(string title)
        {
            _debug($"--- COOKIES ({title}) ---");

            var uri = new Uri("https://rejestr-bdo.mos.gov.pl");
            foreach (Cookie c in Cookies.GetCookies(uri))
            {
                _debug($"{c.Name} = {c.Value}");
            }

            _debug($"--- END COOKIES ({title}) ---");
        }

        private void DumpHtmlSnippet(string html, string title)
        {
            _debug($"--- {title} (pierwsze 1000 znaków) ---");

            var snippet = html.Length > 1000
                ? html.Substring(0, 1000)
                : html;

            _debug(snippet.Replace("\n", "").Replace("\r", ""));

            _debug($"--- END {title} ---");
        }

        private string? ExtractAntiForgeryToken(string html)
        {
            var match = Regex.Match(
                html,
                @"name=""__RequestVerificationToken""\s+type=""hidden""\s+value=""([^""]+)""",
                RegexOptions.IgnoreCase);

            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
