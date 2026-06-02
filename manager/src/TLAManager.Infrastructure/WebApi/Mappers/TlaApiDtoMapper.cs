using TLAManager.Domain;
using TLAManager.Infrastructure.WebApi.Dtos;

namespace TLAManager.Infrastructure.WebApi.Mappers;

public static class TLAApiDtoMapper
{
    public static GroupDto GroupToDto(Group group)
    {
        return new GroupDto(
            group.Name.Name,
            group.Description,
            group.Tlas.Select(tla => new TLADto(tla.Name.Name, tla.Meaning)
                    .WithAlternativeMeanings(tla.AlternativeMeanings)
                    .WithLink(tla.GetAbsoluteUri()))
                .ToList()
        );
    }

    public static Group CreateGroupFromDto(GroupDto groupDto)
    {
        var name = new ShortName(groupDto.Name);
        var tlas = groupDto.Tlas.Select(TlaDtoToTla);
        var tlaGroup = new Group(name, groupDto.Description, tlas);
        return tlaGroup;
    }

    public static ThreeLetterAbbreviation TlaDtoToTla(TLADto tlaDto)
    {
        var shortName = new ShortName(tlaDto.Name);
        var alternativeMeanings = tlaDto.AlternativeMeanings?.ToList() ?? [];
        var defaultStatus = Status.Proposed;
        var tla = new ThreeLetterAbbreviation(shortName, tlaDto.Meaning, alternativeMeanings, tlaDto.Link, defaultStatus);
        return tla;
    }
}