using System.Collections.Immutable;
using TLAManager.Domain.Exceptions;

namespace TLAManager.Domain;

public class Group
{
    public const string CommonGroupName = "common";

    public ShortName Name { get; }

    public string Description { get; }

    private readonly SortedSet<ThreeLetterAbbreviation> _tlas = [];
    public ImmutableSortedSet<ThreeLetterAbbreviation> Tlas => _tlas.ToImmutableSortedSet();

    public Group(ShortName name, string description, IEnumerable<ThreeLetterAbbreviation> tlas)
    {
        Name = name;
        Description = description;

        foreach (var tla in tlas)
        {
            var tryAddResult = _tlas.Add(tla);
            if (!tryAddResult)
            {
                throw new DuplicateTLANameException($"Cannot create TLA group with duplicate TLA '{tla.Name}'");
            }
        }
    }

    public void AddTLA(ThreeLetterAbbreviation tla)
    {
        var tryAddResult = _tlas.Add(tla);
        if (!tryAddResult)
        {
            throw new TLANameAlreadyExistsInGroupException($"TLA '{tla.Name}' already exists in group '{Name.Name}'");
        }
    }

    public void AcceptTLA(ShortName shortName)
    {
        var tla = _tlas.FirstOrDefault(t => t.Name.Equals(shortName));
        if (tla == null)
        {
            throw new TLANameDoesNotExistException($"A TLA '{shortName.Name}' does not exist!");
        }

        tla.Accept();
    }
}