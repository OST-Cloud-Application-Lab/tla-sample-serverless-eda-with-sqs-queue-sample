namespace TLAManager.Infrastructure.Events;

public class TLAReportRequestEventDTO
{
    public required string ReportId { get; set; }

    public required List<TLAGroupEventDto> TLAGroups { get; set; }
}

public class TLAGroupEventDto
{
    public required string Name { get; set; }

    public required string Description { get; set; }

    public required List<TLAEventDto> TLAs { get; set; }
}

public class TLAEventDto
{
    public required string Name { get; set; }

    public required string Meaning { get; set; }

    public required List<string> AlternativeMeanings { get; set; }

    public required string Status { get; set; }

    public string? Link { get; set; }
}
