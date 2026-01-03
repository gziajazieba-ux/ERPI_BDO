using System;
using Newtonsoft.Json;
using System.CodeDom.Compiler;

[GeneratedCode("Manual", "1.0.0")]
public sealed class KpoReceiverFullDetailsDto
{
    // =====================================================
    // LIST – IDENTYFIKACJA
    // =====================================================

    [JsonProperty("KpoId")]
    public Guid? KpoId { get; set; }

    [JsonProperty("KpokId")]
    public Guid? KpokId { get; set; }

    [JsonProperty("CardNumber")]
    public string CardNumber { get; set; }

    [JsonProperty("KpoCardNumber")]
    public string KpoCardNumber { get; set; }

    [JsonProperty("KpokCardNumber")]
    public string KpokCardNumber { get; set; }

    [JsonProperty("Year")]
    public int? Year { get; set; }

    // =====================================================
    // LIST – STATUS
    // =====================================================

    [JsonProperty("Status")]
    public string Status { get; set; }

    [JsonProperty("CardStatusId")]
    public int? CardStatusId { get; set; }

    [JsonProperty("CardStatus")]
    public string CardStatus { get; set; }

    [JsonProperty("CardStatusCodeName")]
    public string CardStatusCodeName { get; set; }

    [JsonProperty("IsWithdrawn")]
    public bool? IsWithdrawn { get; set; }

    // =====================================================
    // LIST – ODPAD
    // =====================================================

    [JsonProperty("WasteCodeId")]
    public int? WasteCodeId { get; set; }

    [JsonProperty("WasteCode")]
    public string WasteCode { get; set; }

    [JsonProperty("WasteCodeDescription")]
    public string WasteCodeDescription { get; set; }

    [JsonProperty("WasteCodeAndDescription")]
    public string WasteCodeAndDescription { get; set; }

    [JsonProperty("DeclaredWasteMass")]
    public double? DeclaredWasteMass { get; set; }

    // =====================================================
    // EFFECTIVE – MASY
    // =====================================================

    [JsonProperty("ReceivedWasteMass")]
    public double? ReceivedWasteMass { get; set; }

    [JsonProperty("RevisedWasteMass")]
    public double? RevisedWasteMass { get; set; }

    [JsonProperty("CorrectedWasteMass")]
    public double? CorrectedWasteMass { get; set; }

    // =====================================================
    // EX / HAZARD
    // =====================================================

    [JsonProperty("WasteCodeExtended")]
    public bool? WasteCodeExtended { get; set; }

    [JsonProperty("WasteCodeExtendedDescription")]
    public string WasteCodeExtendedDescription { get; set; }

    [JsonProperty("HazardousWasteReclassification")]
    public bool? HazardousWasteReclassification { get; set; }

    [JsonProperty("HazardousWasteReclassificationDescription")]
    public string HazardousWasteReclassificationDescription { get; set; }

    // =====================================================
    // SENDER
    // =====================================================

    [JsonProperty("SenderCompanyId")]
    public Guid? SenderCompanyId { get; set; }

    [JsonProperty("SenderCompanyName")]
    public string SenderCompanyName { get; set; }

    [JsonProperty("SenderEupId")]
    public Guid? SenderEupId { get; set; }

    [JsonProperty("SenderIdentificationNumber")]
    public string SenderIdentificationNumber { get; set; }

    [JsonProperty("SenderNip")]
    public string SenderNip { get; set; }

    // =====================================================
    // RECEIVER
    // =====================================================

    [JsonProperty("ReceiverCompanyId")]
    public Guid? ReceiverCompanyId { get; set; }

    [JsonProperty("ReceiverCompanyName")]
    public string ReceiverCompanyName { get; set; }

    [JsonProperty("ReceiverNameOrFirstNameAndLastName")]
    public string ReceiverNameOrFirstNameAndLastName { get; set; }

    [JsonProperty("ReceiverNip")]
    public string ReceiverNip { get; set; }

    [JsonProperty("ReceiverNipEu")]
    public string ReceiverNipEu { get; set; }

    [JsonProperty("ReceiverEupId")]
    public Guid? ReceiverEupId { get; set; }

    [JsonProperty("ReceiverEupNumber")]
    public string ReceiverEupNumber { get; set; }

    [JsonProperty("ReceiverEupName")]
    public string ReceiverEupName { get; set; }

    [JsonProperty("ReceiverEupAddress")]
    public string ReceiverEupAddress { get; set; }

    // =====================================================
    // CARRIER / TRANSPORT
    // =====================================================

    [JsonProperty("CarrierCompanyId")]
    public Guid? CarrierCompanyId { get; set; }

    [JsonProperty("CarrierCompanyName")]
    public string CarrierCompanyName { get; set; }

    [JsonProperty("CarrierNip")]
    public string CarrierNip { get; set; }

    [JsonProperty("CarrierNipEu")]
    public string CarrierNipEu { get; set; }

    [JsonProperty("VehicleRegNumber")]
    public string VehicleRegNumber { get; set; }

    // =====================================================
    // DETAILS – TRANSPORT DATY
    // =====================================================

    [JsonProperty("PlannedTransportTime")]
    public DateTimeOffset? PlannedTransportTime { get; set; }

    [JsonProperty("RealTransportTime")]
    public DateTimeOffset? RealTransportTime { get; set; }

    [JsonProperty("ReceiveConfirmationTime")]
    public DateTimeOffset? ReceiveConfirmationTime { get; set; }

    [JsonProperty("TransportConfirmationTime")]
    public DateTimeOffset? TransportConfirmationTime { get; set; }

    // =====================================================
    // EFFECTIVE – ZATWIERDZENIA
    // =====================================================

    [JsonProperty("CardApprovalTime")]
    public DateTimeOffset? CardApprovalTime { get; set; }

    [JsonProperty("ApprovalUser")]
    public string ApprovalUser { get; set; }

    [JsonProperty("CardWithdrawalTime")]
    public DateTimeOffset? CardWithdrawalTime { get; set; }

    [JsonProperty("WithdrawnByUser")]
    public string WithdrawnByUser { get; set; }

    // =====================================================
    // PROCES / INFO
    // =====================================================

    [JsonProperty("WasteProcessId")]
    public int? WasteProcessId { get; set; }

    [JsonProperty("CertificateNumberAndBoxNumbers")]
    public string CertificateNumberAndBoxNumbers { get; set; }

    [JsonProperty("InstallationName")]
    public string InstallationName { get; set; }

    [JsonProperty("Remarks")]
    public string Remarks { get; set; }

    [JsonProperty("AdditionalInfo")]
    public string AdditionalInfo { get; set; }
}
