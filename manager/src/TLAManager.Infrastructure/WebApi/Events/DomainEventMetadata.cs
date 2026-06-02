namespace TLAManager.Infrastructure.Events;

public class DomainEventMetadata
{
    public string Version { get; set; } = "1.0";

    public string Created_at { get; set; } = string.Empty;

    public DomainEventDomainMetaData? Domain { get; set; } = null;
}