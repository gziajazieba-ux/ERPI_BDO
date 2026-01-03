using System;

namespace ERPI_BDO.Api
{
    public class KpoReceiverFullDetailsDto
    {
        // =================================================
        // IDENTYFIKACJA KARTY (LIST)
        // =================================================
        public Guid List_KpoId { get; set; }
        public string? List_CardNumber { get; set; }
        public int? List_Year { get; set; }
        public string? List_Status { get; set; }
        public string? List_StatusCode { get; set; }

        // =================================================
        // ODPAD (LIST)
        // =================================================
        public string? List_WasteCode { get; set; }
        public string? List_WasteName { get; set; }
        public bool? List_IsHazardousWaste { get; set; }

        // =================================================
        // DATY (LIST)
        // =================================================
        public DateTime? List_PlannedTransportTime { get; set; }
        public DateTime? List_TransportTime { get; set; }
        public DateTime? List_ReceiveConfirmationTime { get; set; }

        // =================================================
        // SENDER (LIST)
        // =================================================
        public Guid? Sender_List_CompanyId { get; set; }
        public Guid? Sender_List_EupId { get; set; }
        public string? Sender_List_CompanyName { get; set; }
        public string? Sender_List_FirstNameLastName { get; set; }

        // =================================================
        // RECEIVER (LIST)
        // =================================================
        public Guid? Receiver_List_CompanyId { get; set; }
        public Guid? Receiver_List_EupId { get; set; }
        public string? Receiver_List_CompanyName { get; set; }
        public string? Receiver_List_FirstNameLastName { get; set; }

        // =================================================
        // CARRIER / TRANSPORTUJ•CY (LIST ñ JEDYNE èR”D£O)
        // =================================================
        public Guid? Carrier_List_CompanyId { get; set; }
        public Guid? Carrier_List_EupId { get; set; }
        public string? Carrier_List_CompanyName { get; set; }
        public string? Carrier_List_FirstNameLastName { get; set; }
        public string? Carrier_List_VehicleRegistrationNumber { get; set; }
        public string? Carrier_List_DriverName { get; set; }

        // =================================================
        // DETAILS ñ RECEIVER
        // =================================================
        public Guid? Receiver_Details_CompanyId { get; set; }
        public Guid? Receiver_Details_EupId { get; set; }
        public string? Receiver_Details_CompanyName { get; set; }
        public string? Receiver_Details_FirstNameLastName { get; set; }
        public string? Receiver_Details_IdentificationNumber { get; set; }
        public string? Receiver_Details_Nip { get; set; }

        // =================================================
        // DETAILS ñ SENDER (JEåLI DOST PNE)
        // =================================================
        public Guid? Sender_Details_CompanyId { get; set; }
        public Guid? Sender_Details_EupId { get; set; }
        public string? Sender_Details_CompanyName { get; set; }
        public string? Sender_Details_FirstNameLastName { get; set; }
        public string? Sender_Details_IdentificationNumber { get; set; }
        public string? Sender_Details_Nip { get; set; }

        // =================================================
        // DETAILS ñ OPERACYJNE KARTY
        // =================================================
        public decimal? Details_WasteMass { get; set; }
        public DateTime? Details_ConfirmationTime { get; set; }
        public string? Details_ConfirmationType { get; set; }

        // =================================================
        // EFFECTIVE / OBLICZENIOWE (NA PRZYSZ£Oå∆)
        // =================================================
        public decimal? Effective_WasteMass { get; set; }
        public bool? Effective_IsCompleted { get; set; }
    }

}
