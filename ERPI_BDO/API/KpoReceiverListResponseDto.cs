using ERPI_BDO.Models;
using System.Collections.Generic;

public class KpoReceiverListResponseDto
{
    public int TotalCount { get; set; }
    public List<KpoReceiverListItemDto> Items { get; set; } = new();
}
