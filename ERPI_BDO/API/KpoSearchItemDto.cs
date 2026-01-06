using System;
using System.Text.Json.Serialization;

namespace ERPI_BDO.Models
{
    public sealed class KpoSearchItemDto
    {
        [JsonPropertyName("kpoId")]
        public Guid KpoId { get; set; }

        [JsonPropertyName("cardNumber")]
        public string? CardNumber { get; set; }

        [JsonPropertyName("cardStatus")]
        public string? CardStatus { get; set; }

        [JsonPropertyName("cardStatusCodeName")]
        public string? CardStatusCodeName { get; set; }

        [JsonPropertyName("senderName")] // W logu jest senderName, nie senderCompanyName
        public string? SenderName { get; set; }

        [JsonPropertyName("receiverName")] // W logu jest receiverName
        public string? ReceiverName { get; set; }

        [JsonPropertyName("wasteCode")]
        public string? WasteCode { get; set; }

        [JsonPropertyName("wasteCodeDescription")]
        public string? WasteCodeDescription { get; set; }

        [JsonPropertyName("vehicleRegNumber")]
        public string? VehicleRegNumber { get; set; }

        [JsonPropertyName("plannedTransportTime")]
        public DateTime? PlannedTransportTime { get; set; }

        [JsonPropertyName("realTransportTime")]
        public DateTime? RealTransportTime { get; set; }

        [JsonPropertyName("receiveConfirmationTime")]
        public DateTime? ReceiveConfirmationTime { get; set; }

        [JsonPropertyName("senderFirstNameAndLastName")]
        public string? SenderPerson { get; set; }

        [JsonPropertyName("receiverFirstAndLastName")]
        public string? ReceiverPerson { get; set; }
    }

    public sealed class KpoSearchResponseDto
    {
        [JsonPropertyName("items")]
        public List<KpoSearchItemDto> Items { get; set; } = new();
    }
}