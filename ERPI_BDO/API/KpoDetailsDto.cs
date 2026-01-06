using System.Text.Json.Serialization;

public sealed class KpoDetailsDto
{
    [JsonPropertyName("kpoId")]
    public Guid KpoId { get; set; }

    // DANE FIRM (To czego brakowało w SEARCH)
    [JsonPropertyName("senderCompanyId")]
    public Guid? SenderCompanyId { get; set; }
    [JsonPropertyName("receiverCompanyId")]
    public Guid? ReceiverCompanyId { get; set; }
    [JsonPropertyName("carrierCompanyId")]
    public Guid? CarrierCompanyId { get; set; }

    // MASY (Źródło prawdy o wadze)
    [JsonPropertyName("wasteMass")]
    public double? WasteMass { get; set; } // Zazwyczaj masa po potwierdzeniu

    [JsonPropertyName("plannedWasteMass")]
    public double? PlannedWasteMass { get; set; } // Masa planowana

    // ADRESY (Jeśli będą potrzebne)
    [JsonPropertyName("senderAdress")]
    public string? SenderAdress { get; set; }
    [JsonPropertyName("receiverAdress")]
    public string? ReceiverAdress { get; set; }
}