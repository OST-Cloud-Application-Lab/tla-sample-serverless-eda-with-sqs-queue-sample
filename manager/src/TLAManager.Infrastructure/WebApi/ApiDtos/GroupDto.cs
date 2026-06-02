namespace TLAManager.Infrastructure.WebApi.Dtos;

public class GroupDto
{
    public string Name { get; }

    public string Description { get; }

    public List<TLADto> Tlas { get; }

    public GroupDto(string name, string description, List<TLADto> tlas)
    {
        Name = name;
        Description = description;
        Tlas = tlas;
    }
}