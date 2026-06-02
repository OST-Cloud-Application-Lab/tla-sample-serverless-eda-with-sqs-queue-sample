namespace TLAManager.Infrastructure.Events;

public class DomainEventDomainMetaData
{
    public string Name { get; set; } = "TLAs";

    public string Subdomain { get; set; } = string.Empty;

    public string Service { get; set; } = string.Empty;

    public string Category { get; set; } = "domain_event";

    public string Event { get; set; } = string.Empty;
}