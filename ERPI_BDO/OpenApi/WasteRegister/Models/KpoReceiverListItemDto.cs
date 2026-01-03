using System;
using Newtonsoft.Json;
using System.CodeDom.Compiler;

[GeneratedCode("NJsonSchema", "13.18.2.0")]
public partial class KpoReceiverListItemDto
{
    // ===== IDENTYFIKACJA =====

    [JsonProperty("KpoId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? KpoId { get; set; }

    [JsonProperty("KpokId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? KpokId { get; set; }

    [JsonProperty("CardNumber", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CardNumber { get; set; }

    [JsonProperty("KpoCardNumber", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string KpoCardNumber { get; set; }

    [JsonProperty("KpokCardNumber", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string KpokCardNumber { get; set; }

    // ===== STATUS =====

    [JsonProperty("Status", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string Status { get; set; }

    [JsonProperty("CardStatus", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CardStatus { get; set; }

    [JsonProperty("CardStatusCodeName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CardStatusCodeName { get; set; }

    [JsonProperty("IsWithdrawn", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsWithdrawn { get; set; }

    // ===== ODPAD =====

    [JsonProperty("WasteCodeId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public int? WasteCodeId { get; set; }

    [JsonProperty("WasteCode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string WasteCode { get; set; }

    [JsonProperty("WasteCodeDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string WasteCodeDescription { get; set; }

    [JsonProperty("WasteCodeAndDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string WasteCodeAndDescription { get; set; }

    [JsonProperty("WasteMass", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string WasteMass { get; set; }

    [JsonProperty("Quantity", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Quantity { get; set; }

    [JsonProperty("ReceivedWasteMass", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public double? ReceivedWasteMass { get; set; }

    // ===== EX / HAZARD =====

    [JsonProperty("WasteCodeExtended", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public bool? WasteCodeExtended { get; set; }

    [JsonProperty("WasteCodeExtendedDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string WasteCodeExtendedDescription { get; set; }

    [JsonProperty("HazardousWasteReclassification", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public bool? HazardousWasteReclassification { get; set; }

    [JsonProperty("HazardousWasteReclassificationDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string HazardousWasteReclassificationDescription { get; set; }

    // ===== PRZEKAZUJĄCY =====

    [JsonProperty("SenderCompanyId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? SenderCompanyId { get; set; }

    [JsonProperty("SenderCompanyName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string SenderCompanyName { get; set; }

    [JsonProperty("SenderEupId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? SenderEupId { get; set; }

    // ===== PRZEJMUJĄCY =====

    [JsonProperty("ReceiverCompanyId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? ReceiverCompanyId { get; set; }

    [JsonProperty("ReceiverNameOrFirstNameAndLastName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ReceiverNameOrFirstNameAndLastName { get; set; }

    [JsonProperty("ReceiverNip", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ReceiverNip { get; set; }

    [JsonProperty("ReceiverNipEu", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ReceiverNipEu { get; set; }

    [JsonProperty("ReceiverEupId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? ReceiverEupId { get; set; }

    [JsonProperty("ReceiverEupNumber", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ReceiverEupNumber { get; set; }

    [JsonProperty("ReceiverEupName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ReceiverEupName { get; set; }

    [JsonProperty("ReceiverEupAddress", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ReceiverEupAddress { get; set; }

    // ===== TRANSPORT =====

    [JsonProperty("CarrierCompanyId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public Guid? CarrierCompanyId { get; set; }

    [JsonProperty("CarrierCompanyName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CarrierCompanyName { get; set; }

    [JsonProperty("CarrierNip", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CarrierNip { get; set; }

    [JsonProperty("CarrierNipEu", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CarrierNipEu { get; set; }

    [JsonProperty("VehicleRegNumber", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string VehicleRegNumber { get; set; }

    [JsonProperty("PlannedTransportTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? PlannedTransportTime { get; set; }

    [JsonProperty("RealTransportTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? RealTransportTime { get; set; }

    // ===== DATY =====

    [JsonProperty("CreatedDate", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? CreatedDate { get; set; }

    [JsonProperty("AcceptanceDate", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? AcceptanceDate { get; set; }

    [JsonProperty("ReceiveDate", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? ReceiveDate { get; set; }

    [JsonProperty("CardApprovalTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? CardApprovalTime { get; set; }

    [JsonProperty("CardWithdrawalTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? CardWithdrawalTime { get; set; }

    // ===== UŻYTKOWNICY =====

    [JsonProperty("CreatedByUser", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string CreatedByUser { get; set; }

    [JsonProperty("ApprovalUser", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ApprovalUser { get; set; }

    [JsonProperty("WithdrawnByUser", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string WithdrawnByUser { get; set; }

    // ===== DODATKOWE =====

    [JsonProperty("InstallationName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string InstallationName { get; set; }

    [JsonProperty("WasteProcessId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public int? WasteProcessId { get; set; }

    [JsonProperty("Remarks", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string Remarks { get; set; }

    [JsonProperty("AdditionalInfo", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string AdditionalInfo { get; set; }
}
