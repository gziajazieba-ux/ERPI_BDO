using System;

namespace ERPI_BDO.Api
{
    public class KpoReceiverFullDetailsDto
    {
        // =========================================================
        // IDENTYFIKACJA KARTY (WSPÓLNA)
        // =========================================================

        public Guid KpoId { get; set; }
        public int? Year { get; set; }
        public string? CardNumber { get; set; }

        // =========================================================
        // ======================= SENDER ==========================
        // =========================================================

        // ---------- LIST ----------
        public Guid? Sender_List_CompanyId { get; set; }
        public Guid? Sender_List_EupId { get; set; }
        public string? Sender_List_CompanyName { get; set; }
        public string? Sender_List_Name { get; set; }
        public string? Sender_List_FirstNameAndLastName { get; set; }

        // ---------- DETAILS ----------
        public Guid? Sender_Details_CompanyId { get; set; }
        public Guid? Sender_Details_EupId { get; set; }
        public string? Sender_Details_CompanyName { get; set; }
        public string? Sender_Details_Name { get; set; }
        public string? Sender_Details_FirstNameAndLastName { get; set; }
        public string? Sender_Details_IdentificationNumber { get; set; }
        public string? Sender_Details_Nip { get; set; }

        public string? Sender_Details_AddressHtml { get; set; }
        public string? Sender_Details_TerytPk { get; set; }
        public string? Sender_Details_PostalCode { get; set; }
        public string? Sender_Details_Locality { get; set; }
        public string? Sender_Details_Street { get; set; }
        public string? Sender_Details_BuildingNumber { get; set; }
        public string? Sender_Details_LocalNumber { get; set; }
        public bool? Sender_Details_HasNoBuildingNumber { get; set; }
        public bool? Sender_Details_ForeignCompany { get; set; }
        public int? Sender_Details_CountryId { get; set; }
        public string? Sender_Details_CountryName { get; set; }

        // =========================================================
        // ======================= CARRIER =========================
        // =========================================================

        // ---------- LIST ----------
        public Guid? Carrier_List_CompanyId { get; set; }
        public Guid? Carrier_List_EupId { get; set; }
        public string? Carrier_List_CompanyName { get; set; }
        public string? Carrier_List_VehicleRegNumber { get; set; }

        // ---------- DETAILS ----------
        public Guid? Carrier_Details_CompanyId { get; set; }
        public Guid? Carrier_Details_EupId { get; set; }
        public string? Carrier_Details_CompanyName { get; set; }

        public string? Carrier_Details_IdentificationNumber { get; set; }
        public string? Carrier_Details_Nip { get; set; }
        public string? Carrier_Details_EuNip { get; set; }
        public string? Carrier_Details_RegistryNumber { get; set; }

        public string? Carrier_Details_AddressHtml { get; set; }
        public string? Carrier_Details_TerytPk { get; set; }
        public string? Carrier_Details_PostalCode { get; set; }
        public string? Carrier_Details_Locality { get; set; }
        public string? Carrier_Details_Street { get; set; }
        public string? Carrier_Details_BuildingNumber { get; set; }
        public string? Carrier_Details_LocalNumber { get; set; }
        public bool? Carrier_Details_HasNoBuildingNumber { get; set; }
        public bool? Carrier_Details_ForeignCompany { get; set; }
        public int? Carrier_Details_CountryId { get; set; }
        public string? Carrier_Details_CountryName { get; set; }

        // =========================================================
        // ====================== RECEIVER =========================
        // =========================================================

        // ---------- LIST ----------
        public Guid? Receiver_List_CompanyId { get; set; }
        public Guid? Receiver_List_EupId { get; set; }
        public string? Receiver_List_CompanyName { get; set; }
        public string? Receiver_List_Name { get; set; }
        public string? Receiver_List_FirstNameAndLastName { get; set; }

        // ---------- DETAILS ----------
        public Guid? Receiver_Details_CompanyId { get; set; }
        public Guid? Receiver_Details_EupId { get; set; }
        public string? Receiver_Details_CompanyName { get; set; }
        public string? Receiver_Details_Name { get; set; }
        public string? Receiver_Details_FirstNameAndLastName { get; set; }
        public string? Receiver_Details_IdentificationNumber { get; set; }
        public string? Receiver_Details_Nip { get; set; }

        public string? Receiver_Details_AddressHtml { get; set; }
        public string? Receiver_Details_TerytPk { get; set; }
        public string? Receiver_Details_PostalCode { get; set; }
        public string? Receiver_Details_Locality { get; set; }
        public string? Receiver_Details_Street { get; set; }
        public string? Receiver_Details_BuildingNumber { get; set; }
        public string? Receiver_Details_LocalNumber { get; set; }
        public bool? Receiver_Details_HasNoBuildingNumber { get; set; }
        public bool? Receiver_Details_ForeignCompany { get; set; }
        public int? Receiver_Details_CountryId { get; set; }
        public string? Receiver_Details_CountryName { get; set; }

        // =========================================================
        // ====================== STATUS / ODPAD ===================
        // =========================================================

        // LIST
        public string? List_CardStatus { get; set; }
        public int? List_CardStatusId { get; set; }
        public string? List_CardStatusCodeName { get; set; }

        // DETAILS
        public string? Details_CardStatus { get; set; }
        public int? Details_CardStatusId { get; set; }
        public string? Details_CardStatusCodeName { get; set; }

        // ODPAD
        public int? WasteCodeId { get; set; }
        public string? WasteCode { get; set; }
        public string? WasteCodeDescription { get; set; }
        public bool? WasteCodeExtended { get; set; }

        // =========================================================
        // ====================== MASY / DATY ======================
        // =========================================================

        // LIST
        public decimal? List_WasteMass { get; set; }
        public DateTime? List_PlannedTransportTime { get; set; }
        public DateTime? List_RealTransportTime { get; set; }
        public DateTime? List_ReceiveConfirmationTime { get; set; }

        // DETAILS
        public decimal? Details_WasteMass { get; set; }
        public decimal? Details_RevisedWasteMass { get; set; }
        public decimal? Details_CorrectedWasteMass { get; set; }

        public DateTime? Details_PlannedTransportTime { get; set; }
        public DateTime? Details_RealTransportTime { get; set; }
        public DateTime? Details_ReceiveConfirmationTime { get; set; }
        public DateTime? Details_TransportConfirmationTime { get; set; }
        public DateTime? Details_CardApprovalTime { get; set; }
        public DateTime? Details_CardRejectionTime { get; set; }

        // =========================================================
        // ======================= EFFECTIVE =======================
        // =========================================================

        public decimal? Effective_WasteMass { get; set; }
        public DateTime? Effective_TransportTime { get; set; }
        public bool Effective_IsCorrected { get; set; }
    }
}
