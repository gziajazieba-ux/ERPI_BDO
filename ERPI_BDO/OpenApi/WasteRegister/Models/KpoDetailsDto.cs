using System;
using System.Text.Json.Serialization;

namespace ERPI_BDO.OpenApi.WasteRegister.Models
{
    public class KpoDetailsDto
    {
        // --- Podstawowe informacje o karcie ---
        [JsonPropertyName("kpoId")] public Guid KpoId { get; set; }
        [JsonPropertyName("cardNumber")] public string? CardNumber { get; set; }
        [JsonPropertyName("year")] public int? Year { get; set; }
        [JsonPropertyName("isRevised")] public bool? IsRevised { get; set; }

        // --- Statusy ---
        [JsonPropertyName("cardStatusId")] public int? CardStatusId { get; set; }
        [JsonPropertyName("cardStatus")] public string? CardStatus { get; set; }
        [JsonPropertyName("cardStatusCodeName")] public string? CardStatusCodeName { get; set; }

        // --- Nadawca (Sender) ---
        [JsonPropertyName("senderCompanyId")] public Guid? SenderCompanyId { get; set; }
        [JsonPropertyName("senderEupId")] public Guid? SenderEupId { get; set; }
        [JsonPropertyName("senderName")] public string? SenderName { get; set; }
        [JsonPropertyName("senderCompanyName")] public string? SenderCompanyName { get; set; }
        [JsonPropertyName("senderFirstNameAndLastName")] public string? SenderFirstNameAndLastName { get; set; }
        [JsonPropertyName("senderAddress")] public string? SenderAddress { get; set; }
        [JsonPropertyName("senderIdentificationNumber")] public string? SenderIdentificationNumber { get; set; }
        [JsonPropertyName("senderNip")] public string? SenderNip { get; set; }
        [JsonPropertyName("senderNipEu")] public string? SenderNipEu { get; set; }
        [JsonPropertyName("senderEupNumber")] public string? SenderEupNumber { get; set; }
        [JsonPropertyName("senderEupName")] public string? SenderEupName { get; set; }
        [JsonPropertyName("senderEupAddress")] public string? SenderEupAddress { get; set; }

        // --- Odbiorca (Receiver) ---
        [JsonPropertyName("receiverCompanyId")] public Guid? ReceiverCompanyId { get; set; }
        [JsonPropertyName("receiverEupId")] public Guid? ReceiverEupId { get; set; }
        [JsonPropertyName("receiverName")] public string? ReceiverName { get; set; }
        [JsonPropertyName("receiverCompanyName")] public string? ReceiverCompanyName { get; set; }
        [JsonPropertyName("receiverFirstNameAndLastName")] public string? ReceiverFirstNameAndLastName { get; set; }
        [JsonPropertyName("receiverAddress")] public string? ReceiverAddress { get; set; }
        [JsonPropertyName("receiverIdentificationNumber")] public string? ReceiverIdentificationNumber { get; set; }
        [JsonPropertyName("receiverNip")] public string? ReceiverNip { get; set; }
        [JsonPropertyName("receiverNipEu")] public string? ReceiverNipEu { get; set; }
        [JsonPropertyName("receiverEupNumber")] public string? ReceiverEupNumber { get; set; }
        [JsonPropertyName("receiverEupName")] public string? ReceiverEupName { get; set; }
        [JsonPropertyName("receiverEupAddress")] public string? ReceiverEupAddress { get; set; }

        // --- Transportuj¹cy (Carrier) ---
        [JsonPropertyName("carrierCompanyId")] public Guid? CarrierCompanyId { get; set; }
        [JsonPropertyName("carrierName")] public string? CarrierName { get; set; }
        [JsonPropertyName("carrierCompanyName")] public string? CarrierCompanyName { get; set; }
        [JsonPropertyName("carrierFirstNameAndLastName")] public string? CarrierFirstNameAndLastName { get; set; }
        [JsonPropertyName("carrierAddress")] public string? CarrierAddress { get; set; }
        [JsonPropertyName("carrierIdentificationNumber")] public string? CarrierIdentificationNumber { get; set; }
        [JsonPropertyName("carrierNip")] public string? CarrierNip { get; set; }
        [JsonPropertyName("carrierNipEu")] public string? CarrierNipEu { get; set; }

        // --- Dane o odpadach ---
        [JsonPropertyName("wasteCodeId")] public int? WasteCodeId { get; set; }
        [JsonPropertyName("wasteCode")] public string? WasteCode { get; set; }
        [JsonPropertyName("wasteCodeAndDescription")] public string? WasteCodeAndDescription { get; set; }
        [JsonPropertyName("wasteMass")] public decimal? WasteMass { get; set; }
        [JsonPropertyName("wasteProcessId")] public int? WasteProcessId { get; set; }
        [JsonPropertyName("wasteProcess")] public string? WasteProcess { get; set; }
        [JsonPropertyName("wasteProcessWithCode")] public string? WasteProcessWithCode { get; set; }

        // --- Transport i Logistyka ---
        [JsonPropertyName("vehicleRegNumber")] public string? VehicleRegNumber { get; set; }
        [JsonPropertyName("realTransportDate")] public string? RealTransportDate { get; set; }
        [JsonPropertyName("realTransportTime")] public string? RealTransportTime { get; set; }
        [JsonPropertyName("plannedTransportTime")] public DateTime? PlannedTransportTime { get; set; }

        // --- Potwierdzenia i Zatwierdzenia ---
        [JsonPropertyName("approvalDate")] public string? ApprovalDate { get; set; }
        [JsonPropertyName("approvalTime")] public string? ApprovalTime { get; set; }
        [JsonPropertyName("approvedByUser")] public string? ApprovedByUser { get; set; }

        [JsonPropertyName("receiveConfirmationDate")] public string? ReceiveConfirmationDate { get; set; }
        [JsonPropertyName("receiveConfirmationTime")] public string? ReceiveConfirmationTime { get; set; }
        [JsonPropertyName("receiveConfirmedByUser")] public string? ReceiveConfirmedByUser { get; set; }

        [JsonPropertyName("rejectedByUser")] public string? RejectedByUser { get; set; }

        // --- Informacje Dodatkowe i Reasygnacja ---
        [JsonPropertyName("hazardousWasteReclassification")] public bool? HazardousWasteReclassification { get; set; }
        [JsonPropertyName("hazardousWasteReclassificationDescription")] public string? HazardousWasteReclassificationDescription { get; set; }
        [JsonPropertyName("wasteCodeExtended")] public bool? WasteCodeExtended { get; set; }
        [JsonPropertyName("wasteCodeExtendedDescription")] public string? WasteCodeExtendedDescription { get; set; }
        [JsonPropertyName("isWasteGenerating")] public bool? IsWasteGenerating { get; set; }
        [JsonPropertyName("wasteGeneratedTerytPk")] public string? WasteGeneratedTerytPk { get; set; }
        [JsonPropertyName("wasteGeneratingAdditionalInfo")] public string? WasteGeneratingAdditionalInfo { get; set; }

        [JsonPropertyName("certificateNumberAndBoxNumbers")] public string? CertificateNumberAndBoxNumbers { get; set; }
        [JsonPropertyName("certificateNumber")] public string? CertificateNumber { get; set; }
        [JsonPropertyName("additionalInfo")] public string? AdditionalInfo { get; set; }
        [JsonPropertyName("remarks")] public string? Remarks { get; set; }

        // --- Dane adresowe (Metadane/Szczegó³owe) ---
        [JsonPropertyName("identificationNumber")] public string? IdentificationNumber { get; set; }
        [JsonPropertyName("nip")] public string? Nip { get; set; }
        [JsonPropertyName("addressHtml")] public string? AddressHtml { get; set; }
        [JsonPropertyName("postalCode")] public string? PostalCode { get; set; }
        [JsonPropertyName("countryName")] public string? CountryName { get; set; }
        [JsonPropertyName("locality")] public string? Locality { get; set; }
        [JsonPropertyName("street")] public string? Street { get; set; }
        [JsonPropertyName("buildingNumber")] public string? BuildingNumber { get; set; }
        [JsonPropertyName("localNumber")] public string? LocalNumber { get; set; }
    }
}