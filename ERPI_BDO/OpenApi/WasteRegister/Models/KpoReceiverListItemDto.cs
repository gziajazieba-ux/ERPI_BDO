using System;

public class KpoReceiverListItemDto
{
    public string KpoNumber { get; set; } = string.Empty;
    public string WasteCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime? AcceptanceDate { get; set; }
}
