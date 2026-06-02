using System.Collections.Immutable;
using TLAManager.Domain.Exceptions;

namespace TLAManager.Domain;

public class ThreeLetterAbbreviation : IComparable<ThreeLetterAbbreviation>
{
    public ShortName Name { get; }

    public string Meaning { get; }

    private readonly HashSet<string> _alternativeMeanings;
    public ImmutableHashSet<string> AlternativeMeanings => _alternativeMeanings.ToImmutableHashSet();

    private readonly Uri? _uri;

    public Status Status { get; private set; }

    public ThreeLetterAbbreviation(ShortName name, string meaning, IEnumerable<string> alternativeMeanings, string? link, Status status)
    {
        if (string.IsNullOrEmpty(meaning))
        {
            throw new ArgumentException("A TLA's meaning cannot be empty.", nameof(meaning));
        }

        if (!string.IsNullOrEmpty(link))
        {
            if (!Uri.TryCreate(link, UriKind.Absolute, out _uri))
            {
                throw new ArgumentException("The passed link is not a valid URL.", nameof(link));
            }
        }

        Name = name;
        Meaning = meaning;
        Status = status;
        _alternativeMeanings = new HashSet<string>(alternativeMeanings.Where(s => !string.IsNullOrEmpty(s)));

    }

    public void Accept()
    {
        if (Status != Status.Proposed)
            throw new InvalidTLAStateTransitionException("A TLA has to be in state PROPOSED to be accepted.");

        Status = Status.Accepted;
    }

    public void Decline()
    {
        if (Status != Status.Proposed)
            throw new InvalidTLAStateTransitionException("A TLA has to be in state PROPOSED to be declined.");

        Status = Status.Declined;
    }

    public void Archive()
    {
        if (Status != Status.Accepted)
            throw new InvalidTLAStateTransitionException("A TLA has to be in state ACCEPTED to be archived.");

        Status = Status.Archived;
    }

    public string? GetAbsoluteUri()
    {
        return _uri?.AbsoluteUri;
    }

    public int CompareTo(ThreeLetterAbbreviation? other)
    {
        return Name.CompareTo(other?.Name);
    }
}