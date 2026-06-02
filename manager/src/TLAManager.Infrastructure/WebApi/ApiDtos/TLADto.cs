using System.Text.Json.Serialization;

namespace TLAManager.Infrastructure.WebApi.Dtos;

public class TLADto(string name, string meaning, ISet<string> alternativeMeanings, string? link)
{
    public string Name { get; } = name;

    public string Meaning { get; } = meaning;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ISet<string>? AlternativeMeanings { get; } = alternativeMeanings;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Link { get; } = link;
}