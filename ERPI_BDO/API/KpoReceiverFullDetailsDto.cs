using System;
using Newtonsoft.Json;
using System.CodeDom.Compiler;

[GeneratedCode("Manual", "1.0.0")]
public sealed class KpoReceiverFullDetailsDto
{
    // =====================================================
    // IDENTYFIKACJA KARTY (SEARCH)
    // =====================================================
    public Guid? CardId { get; set; }
    public int? Year { get; set; }

    public string? CardNumber { get; set; }
    public string? KpoCardNumber { get; set; }
    public string? KpokCardNumber { get; set; }

    // =====================================================
    // STATUS KARTY (SEARCH)
    // =====================================================
    public int? CardStatusId { get; set; }
    public string? Status { get; set; }
    public string? StatusCode { get; set; }
    public string? CardStatus { get; set; }
    public string? CardStatusCodeName { get; set; }

    public bool? IsWithdrawn { get; set; }

    // =====================================================
    // ODPAD (SEARCH)
    // =====================================================
    public int? WasteCodeId { get; set; }
    public string? WasteCode { get; set; }
    public string? WasteCodeDescription { get; set; }
    public string? WasteCodeAndDescription { get; set; }

    // =====================================================
    // MASY – LISTA (SEARCH)
    // =====================================================
    /// <summary>
    /// Masa deklarowana z SEARCH (quantity)
    /// </summary>
    public double? DeclaredWasteMass { get; set; }

    // =====================================================
    // PODMIOTY – SEARCH (ID + PODSTAWOWE DANE)
    // =====================================================
    // PRZEKAZUJ¥CY
    public Guid? SenderCompanyId { get; set; }
    public string? SenderCompanyName { get; set; }
    public string? SenderNip { get; set; }
    public string? SenderNipEu { get; set; }

    // PRZEJMUJ¥CY
    public Guid? ReceiverCompanyId { get; set; }
    public string? ReceiverCompanyName { get; set; }
    public string? ReceiverNip { get; set; }
    public string? ReceiverNipEu { get; set; }

    // TRANSPORTUJ¥CY
    public Guid? CarrierCompanyId { get; set; }
    public string? CarrierCompanyName { get; set; }
    public string? CarrierNip { get; set; }
    public string? CarrierNipEu { get; set; }

    // =====================================================
    // TRANSPORT – SEARCH
    // =====================================================
    public string? VehicleRegNumber { get; set; }

    // =====================================================
    // DATY – SEARCH
    // =====================================================
    public DateTimeOffset? TransportStartTime { get; set; }

    // =====================================================
    // KONTEKST TECHNICZNY
    // =====================================================
    /// <summary>
    /// 0 – Sender, 1 – Carrier, 2 – Receiver
    /// </summary>
    public int CompanyType { get; set; }

    // =====================================================
    // =================== DETAILS (ETAP 2) =================
    // =====================================================
    // Poni¿sze pola NIE S¥ teraz wype³niane.
    // S¹ zostawione œwiadomie, aby:
    // - nie refaktoryzowaæ DTO
    // - nie ³amaæ przysz³ej logiki mas i dat
    // =====================================================

    // ===== MASY – DETAILS =====
    public double? ReceivedWasteMass { get; set; }
    public double? RevisedWasteMass { get; set; }
    public double? CorrectedWasteMass { get; set; }
    public double? EffectiveWasteMass { get; set; }

    // ===== DATY – DETAILS =====
    public DateTimeOffset? PlannedTransportTime { get; set; }
    public DateTimeOffset? RealTransportTime { get; set; }
    public DateTimeOffset? ReceiveConfirmationTime { get; set; }
    public DateTimeOffset? TransportConfirmationTime { get; set; }

    // ===== U¯YTKOWNICY – DETAILS =====
    public string? GeneratedByUser { get; set; }
    public string? ApprovedByUser { get; set; }
    public string? WithdrawnByUser { get; set; }

    // ===== DODATKOWE – DETAILS =====
    public string? Remarks { get; set; }
    public string? AdditionalInfo { get; set; }
}