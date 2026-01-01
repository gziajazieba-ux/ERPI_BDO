using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class KpoSearchResult
{
    public int pageSize { get; set; }
    public int pageNumber { get; set; }
    public int totalPagesNumber { get; set; }
    public int totalResultNumber { get; set; }
    public bool hasPreviousPage { get; set; }
    public bool hasNextPage { get; set; }
    public List<KpoItem> items { get; set; }
}

public class KpoItem
{
    public string kpoId { get; set; }
    public string cardNumber { get; set; }
    public string cardStatus { get; set; }
    public string cardStatusCodeName { get; set; }

    public string wasteCode { get; set; }
    public string wasteCodeDescription { get; set; }

    public string senderName { get; set; }
    public string receiverName { get; set; }
    public string vehicleRegNumber { get; set; }

    public DateTime? plannedTransportTime { get; set; }
    public DateTime? realTransportTime { get; set; }
    public DateTime? receiveConfirmationTime { get; set; }
    public DateTime? kpoLastModifiedAt { get; set; }

    public string senderFirstNameAndLastName { get; set; }
    public string receiverFirstAndLastName { get; set; }

    public int revisionNumber { get; set; }
    public DateTime? cardRejectionTime { get; set; }
    public string rejectedByUserFirstNameAndLastName { get; set; }

    public bool wasteCodeExtended { get; set; }
    public bool hazardousWasteReclassification { get; set; }
}