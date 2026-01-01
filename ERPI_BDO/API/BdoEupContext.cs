using System;
using System.Net.Http;

public class BdoEupContext
{
    public Guid EupId { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; }

    public string EupToken { get; set; } = string.Empty;
    public HttpClient ApiClient { get; set; } = null!;
}
