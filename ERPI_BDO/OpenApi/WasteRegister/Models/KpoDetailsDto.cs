using System;
using System.Text.Json.Serialization;

namespace ERPI_BDO.OpenApi.WasteRegister.Models
{
    public class KpoDetailsDto
    {
        // =========================================================
        // IDENTYFIKACJA
        // =========================================================

        [JsonPropertyName("kpoId")]
        public Guid KpoId { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("cardNumber")]
        public string? CardNumber { get; set; }

        // =========================================================
        // STATUS
        // =========================================================

        [JsonPropertyName("cardStatusId")]
        public int? CardStatusId { get; set; }

        [JsonPropertyName("cardStatus")]
        public string? CardStatus { get; set; }

        [JsonPropertyName("cardStatusCodeName")]
        public string? CardStatusCodeName { get; set; }

        // =========================================================
        // SENDER
        // =========================================================

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

        // =========================================================
        // RECEIVER
        // =========================================================

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

        // =========================================================
        // CARRIER (TRANSPORTUJ¥CY) – ROZSZERZONE
        // =========================================================

        [JsonPropertyName("carrierCompanyId")]
        public Guid? CarrierCompanyId { get; set; }

        [JsonPropertyName("carrierEupId")]
        public Guid? CarrierEupId { get; set; }

        [JsonPropertyName("carrierCompanyName")]
        public string? CarrierCompanyName { get; set; }

        [JsonPropertyName("vehicleRegNumber")]
        public string? VehicleRegNumber { get; set; }

        // --- KLUCZOWE BRAKUJ¥CE POLA ---
        // (BDO zwraca je w company{} dla CompanyType = Carrier)

        public string? IdentificationNumber { get; set; }   // numer rejestrowy BDO
        public string? Nip { get; set; }
        public string? EuNip { get; set; }
        public string? RegistrationNumber { get; set; }

        // =========================================================
        // ODPAD
        // =========================================================

        [JsonPropertyName("wasteCodeId")]
        public int? WasteCodeId { get; set; }

        [JsonPropertyName("wasteCode")]
        public string? WasteCode { get; set; }

        [JsonPropertyName("wasteCodeDescription")]
        public string? WasteCodeDescription { get; set; }

        [JsonPropertyName("wasteCodeExtended")]
        public bool? WasteCodeExtended { get; set; }

        [JsonPropertyName("wasteCodeExtendedDescription")]
        public string? WasteCodeExtendedDescription { get; set; }

        [JsonPropertyName("hazardousWasteReclassification")]
        public bool? HazardousWasteReclassification { get; set; }

        [JsonPropertyName("hazardousWasteReclassificationDescription")]
        public string? HazardousWasteReclassificationDescription { get; set; }

        // =========================================================
        // MASY
        // =========================================================

        [JsonPropertyName("wasteMass")]
        public decimal? WasteMass { get; set; }

        [JsonPropertyName("revisedWasteMass")]
        public decimal? RevisedWasteMass { get; set; }

        [JsonPropertyName("correctedWasteMass")]
        public decimal? CorrectedWasteMass { get; set; }

        // =========================================================
        // DATY / CZASY
        // =========================================================

        [JsonPropertyName("plannedTransportTime")]
        public DateTime? PlannedTransportTime { get; set; }

        [JsonPropertyName("realTransportTime")]
        public DateTime? RealTransportTime { get; set; }

        [JsonPropertyName("receiveConfirmationTime")]
        public DateTime? ReceiveConfirmationTime { get; set; }

        [JsonPropertyName("transportConfirmationTime")]
        public DateTime? TransportConfirmationTime { get; set; }

        [JsonPropertyName("cardApprovalTime")]
        public DateTime? CardApprovalTime { get; set; }

        [JsonPropertyName("cardRejectionTime")]
        public DateTime? CardRejectionTime { get; set; }

        // =========================================================
        // WYTWARZANIE ODPADU
        // =========================================================

        [JsonPropertyName("isWasteGenerating")]
        public bool? IsWasteGenerating { get; set; }

        [JsonPropertyName("wasteGeneratedTeryt")]
        public string? WasteGeneratedTeryt { get; set; }

        [JsonPropertyName("wasteGeneratedTerytPk")]
        public string? WasteGeneratedTerytPk { get; set; }

        [JsonPropertyName("wasteGeneratingAdditionalInfo")]
        public string? WasteGeneratingAdditionalInfo { get; set; }

        // =========================================================
        // ADRES / KRAJ
        // =========================================================

        [JsonPropertyName("addressHtml")]
        public string? AddressHtml { get; set; }

        [JsonPropertyName("terytPk")]
        public string? TerytPk { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("locality")]
        public string? Locality { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("buildingNumber")]
        public string? BuildingNumber { get; set; }

        [JsonPropertyName("localNumber")]
        public string? LocalNumber { get; set; }

        [JsonPropertyName("hasNoBuildingNumber")]
        public bool? HasNoBuildingNumber { get; set; }

        [JsonPropertyName("foreignCompany")]
        public bool? ForeignCompany { get; set; }

        [JsonPropertyName("countryId")]
        public int? CountryId { get; set; }

        [JsonPropertyName("countryName")]
        public string? CountryName { get; set; }

        // =========================================================
        // REWIZJE / ODRZUCENIA
        // =========================================================

        [JsonPropertyName("isRevised")]
        public bool? IsRevised { get; set; }

        [JsonPropertyName("revisedAt")]
        public DateTime? RevisedAt { get; set; }

        [JsonPropertyName("revisedBy")]
        public string? RevisedBy { get; set; }

        [JsonPropertyName("rejectedByUser")]
        public string? RejectedByUser { get; set; }

        // =========================================================
        // DODATKOWE
        // =========================================================

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }

        [JsonPropertyName("additionalInfo")]
        public string? AdditionalInfo { get; set; }
    }
}
