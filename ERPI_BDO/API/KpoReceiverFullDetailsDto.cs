using System;

namespace ERPI_BDO.Api
{
    public class KpoReceiverFullDetailsDto
    {
        public Guid KpoId { get; set; }
        public int? Year { get; set; }
        public string? CardNumber { get; set; }
        public string? CardStatus { get; set; }
        public int? CardStatusId { get; set; }
        public string? CardStatusCodeName { get; set; }

        // Sender
        public Guid? SenderCompanyId { get; set; }
        public Guid? SenderEupId { get; set; }
        public string? SenderCompanyName { get; set; }
        public string? SenderFirstNameAndLastName { get; set; }
        public string? SenderName { get; set; }

        // Receiver
        public Guid? ReceiverCompanyId { get; set; }
        public Guid? ReceiverEupId { get; set; }
        public string? ReceiverCompanyName { get; set; }
        public string? ReceiverFirstAndLastName { get; set; }
        public string? ReceiverName { get; set; }

        // Carrier
        public Guid? CarrierCompanyId { get; set; }
        public Guid? CarrierEupId { get; set; }
        public string? CarrierCompanyName { get; set; }

        // MASS FIELDS — MUSZ¥ BYÆ
        public int? WasteCodeId { get; set; }
        public string? WasteCode { get; set; }
        public string? WasteCodeDescription { get; set; }

        // wszystkie trzy masy — nie usuwaj/nazwij inaczej
        public decimal? WasteMass { get; set; }             // aktualna masa (w JSON: wasteMass)
        public decimal? CorrectedWasteMass { get; set; }    // masa po korekcie (correctedWasteMass)
        public decimal? RevisedWasteMass { get; set; }      // masa przed korekt¹ (revisedWasteMass)

        public int? RevisedWasteCodeId { get; set; }
        public bool? WasteCodeExtended { get; set; }
        public string? WasteCodeExtendedDescription { get; set; }
        public bool? IsWasteGenerating { get; set; }
        public string? WasteGeneratingAdditionalInfo { get; set; }
        public string? WasteGeneratedTerytPk { get; set; }
        public int? WasteProcessId { get; set; }

        // Revisions/meta
        public bool? IsRevised { get; set; }
        public DateTime? RevisedAt { get; set; }
        public string? RevisedBy { get; set; }

        // Dates / users
        public DateTime? PlannedTransportTime { get; set; }
        public DateTime? RealTransportTime { get; set; }
        public DateTime? ReceiveConfirmationTime { get; set; }
        public DateTime? TransportConfirmationTime { get; set; }
        public DateTime? CardApprovalTime { get; set; }
        public DateTime? CardRejectionTime { get; set; }
        public DateTime? GeneratingConfirmationTime { get; set; }

        public string? ApprovalUser { get; set; }
        public string? TransportConfirmationUser { get; set; }
        public string? GeneratingConfirmationUser { get; set; }
        public string? ReceiveConfirmationUser { get; set; }
        public string? RejectedByUser { get; set; }

        public string? VehicleRegNumber { get; set; }
        public string? CertificateNumberAndBoxNumbers { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool? HazardousWasteReclassification { get; set; }
        public string? HazardousWasteReclassificationDescription { get; set; }
        public string? Remarks { get; set; }

        // Metadata / address
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? Nip { get; set; }
        public string? AddressHtml { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryName { get; set; }
        public string? Locality { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? LocalNumber { get; set; }
        public string? AdditionalInfoForUi { get; set; }

        // G³ówna kolumna masy widoczna w gridzie; mapowanie ustawiane w FormGlowne
        public decimal Quantity { get; set; }

        public KpoReceiverFullDetailsDto() { }
    }
}