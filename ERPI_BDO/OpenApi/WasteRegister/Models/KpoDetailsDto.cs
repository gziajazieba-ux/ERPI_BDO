using System;
using System.Text.Json.Serialization;

namespace ERPI_BDO.OpenApi.WasteRegister.Models
{
    public sealed class KpoDetailsDto
    {
        // =====================================================
        // IDENTYFIKACJA
        // =====================================================

        [JsonPropertyName("kpoId")]
        public Guid? KpoId { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("cardNumber")]
        public string? CardNumber { get; set; }

        // =====================================================
        // STATUS
        // =====================================================

        [JsonPropertyName("cardStatusId")]
        public int? CardStatusId { get; set; }

        [JsonPropertyName("cardStatus")]
        public string? CardStatus { get; set; }

        [JsonPropertyName("cardStatusCodeName")]
        public string? CardStatusCodeName { get; set; }

        // =====================================================
        // SENDER
        // =====================================================

        [JsonPropertyName("senderCompanyId")]
        public Guid? SenderCompanyId { get; set; }

        [JsonPropertyName("senderEupId")]
        public Guid? SenderEupId { get; set; }

        [JsonPropertyName("senderCompanyName")]
        public string? SenderCompanyName { get; set; }

        [JsonPropertyName("senderFirstNameAndLastName")]
        public string? SenderFirstNameAndLastName { get; set; }

        [JsonPropertyName("senderName")]
        public string? SenderName { get; set; }

        [JsonPropertyName("senderIdentificationNumber")]
        public string? SenderIdentificationNumber { get; set; }

        [JsonPropertyName("senderNip")]
        public string? SenderNip { get; set; }

        // =====================================================
        // RECEIVER
        // =====================================================

        [JsonPropertyName("receiverCompanyId")]
        public Guid? ReceiverCompanyId { get; set; }

        [JsonPropertyName("receiverEupId")]
        public Guid? ReceiverEupId { get; set; }

        [JsonPropertyName("receiverCompanyName")]
        public string? ReceiverCompanyName { get; set; }

        [JsonPropertyName("receiverFirstNameAndLastName")]
        public string? ReceiverFirstNameAndLastName { get; set; }

        [JsonPropertyName("receiverName")]
        public string? ReceiverName { get; set; }

        [JsonPropertyName("receiverIdentificationNumber")]
        public string? ReceiverIdentificationNumber { get; set; }

        [JsonPropertyName("receiverNip")]
        public string? ReceiverNip { get; set; }

        // =====================================================
        // CARRIER
        // =====================================================

        [JsonPropertyName("carrierCompanyId")]
        public Guid? CarrierCompanyId { get; set; }

        [JsonPropertyName("carrierEupId")]
        public Guid? CarrierEupId { get; set; }

        [JsonPropertyName("carrierCompanyName")]
        public string? CarrierCompanyName { get; set; }

        [JsonPropertyName("carrierIdentificationNumber")]
        public string? CarrierIdentificationNumber { get; set; }

        [JsonPropertyName("carrierNip")]
        public string? CarrierNip { get; set; }

        [JsonPropertyName("carrierEuNip")]
        public string? CarrierEuNip { get; set; }

        [JsonPropertyName("carrierRegistrationNumber")]
        public string? CarrierRegistrationNumber { get; set; }

        [JsonPropertyName("vehicleRegNumber")]
        public string? VehicleRegNumber { get; set; }

        // =====================================================
        // ODPAD
        // =====================================================

        [JsonPropertyName("wasteCodeId")]
        public int? WasteCodeId { get; set; }

        [JsonPropertyName("wasteCode")]
        public string? WasteCode { get; set; }

        [JsonPropertyName("wasteCodeDescription")]
        public string? WasteCodeDescription { get; set; }

        [JsonPropertyName("wasteMass")]
        public double? WasteMass { get; set; }

        [JsonPropertyName("revisedWasteMass")]
        public double? RevisedWasteMass { get; set; }

        [JsonPropertyName("correctedWasteMass")]
        public double? CorrectedWasteMass { get; set; }

        // --- EX / NIEBEZPIECZNE ---
        [JsonPropertyName("wasteCodeExtended")]
        public bool? WasteCodeExtended { get; set; }

        [JsonPropertyName("wasteCodeExtendedDescription")]
        public string? WasteCodeExtendedDescription { get; set; }

        [JsonPropertyName("hazardousWasteReclassification")]
        public bool? HazardousWasteReclassification { get; set; }

        [JsonPropertyName("hazardousWasteReclassificationDescription")]
        public string? HazardousWasteReclassificationDescription { get; set; }

        // =====================================================
        // TRANSPORT – DATY / STATUSY
        // =====================================================

        [JsonPropertyName("plannedTransportTime")]
        public DateTimeOffset? PlannedTransportTime { get; set; }

        [JsonPropertyName("realTransportTime")]
        public DateTimeOffset? RealTransportTime { get; set; }

        [JsonPropertyName("receiveConfirmationTime")]
        public DateTimeOffset? ReceiveConfirmationTime { get; set; }

        [JsonPropertyName("receiveConfirmationTimeFromKpo")]
        public DateTimeOffset? ReceiveConfirmationTimeFromKpo { get; set; }

        [JsonPropertyName("transportConfirmationTime")]
        public DateTimeOffset? TransportConfirmationTime { get; set; }

        [JsonPropertyName("cardApprovalTime")]
        public DateTimeOffset? CardApprovalTime { get; set; }

        [JsonPropertyName("approvalUser")]
        public string? ApprovalUser { get; set; }        

        [JsonPropertyName("cardRejectionTime")]
        public string? CardRejectionTimeRaw { get; set; }

        [JsonPropertyName("cardWithdrawalTime")]
        public string? CardWithdrawalTimeRaw { get; set; }

        [JsonPropertyName("withdrawnByUser")]
        public string? WithdrawnByUser { get; set; }

        [JsonPropertyName("generatingConfirmationTime")]
        public string? GeneratingConfirmationTimeRaw { get; set; }

        [JsonPropertyName("generatingConfirmationUser")]
        public string? GeneratingConfirmationUser { get; set; }


        // =====================================================
        // TRANSPORT – RODZAJ
        // =====================================================

        [JsonPropertyName("isRoadTransport")]
        public bool? IsRoadTransport { get; set; }

        [JsonPropertyName("isRailwayTransport")]
        public bool? IsRailwayTransport { get; set; }

        [JsonPropertyName("isMaritimeTransport")]
        public bool? IsMaritimeTransport { get; set; }

        [JsonPropertyName("isAirTransport")]
        public bool? IsAirTransport { get; set; }

        [JsonPropertyName("isInlandWaterTransport")]
        public bool? IsInlandWaterTransport { get; set; }

        // =====================================================
        // PROCES / CERTYFIKATY
        // =====================================================

        [JsonPropertyName("wasteProcessId")]
        public int? WasteProcessId { get; set; }

        [JsonPropertyName("certificateNumberAndBoxNumbers")]
        public string? CertificateNumberAndBoxNumbers { get; set; }

        // =====================================================
        // UWAGI / INFO
        // =====================================================

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }

        [JsonPropertyName("additionalInfo")]
        public string? AdditionalInfo { get; set; }
    }
}
