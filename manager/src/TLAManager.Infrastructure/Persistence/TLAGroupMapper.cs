using Amazon.DynamoDBv2.Model;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public static class TLAGroupMapper
{
    private static readonly string NameField = "name";
    private static readonly string DescriptionField = "description";
    private static readonly string TlasField = "tlas";
    private static readonly string MeaningField = "meaning";
    private static readonly string AlternativeMeaningsField = "alternative_meanings";
    private static readonly string UrlField = "url";
    private static readonly string StatusField = "status";

    public static TLAGroup TlaGroupFromDynamoDb(Dictionary<string, AttributeValue> items)
    {
        var builder = new TLAGroup(
            new ShortName(items[NameField].S),
            items[DescriptionField].S,
            items[TlasField].L.Select(av =>
            {
                var tlaMap = av.M;

                List<string> alternativeMeanings = [];
                if (tlaMap.TryGetValue(AlternativeMeaningsField, out var alternativeMeaningsValue))
                {
                    alternativeMeanings = alternativeMeaningsValue.L.Select(v => v.S).ToList();
                }

                var link = tlaMap.TryGetValue(UrlField, out var linkValue) ? linkValue.S : null;
                var status = Enum.Parse<TLAStatus>(tlaMap[StatusField].S);
                var tla = new ThreeLetterAbbreviation(
                    new ShortName(tlaMap[NameField].S),
                    tlaMap[MeaningField].S,
                    alternativeMeanings,
                    link,
                    status
                );
                return tla;
            }).ToList()
        );

        return builder;
    }

    public static Dictionary<string, AttributeValue> TlaGroupToDynamoDb(TLAGroup group)
    {
        var map = new Dictionary<string, AttributeValue>
        {
            { NameField, new AttributeValue { S = group.Name.Name } },
            { DescriptionField, new AttributeValue { S = group.Description } },
            {
                TlasField, new AttributeValue
                {
                    L = group.Tlas.Select(tla =>
                    {
                        var tlaMap = new Dictionary<string, AttributeValue>
                        {
                            { NameField, new AttributeValue { S = tla.Name.Name } },
                            { MeaningField, new AttributeValue { S = tla.Meaning } },
                            { StatusField, new AttributeValue { S = tla.Status.ToString() } }
                        };

                        if (!tla.AlternativeMeanings.IsEmpty)
                        {
                            tlaMap.Add(AlternativeMeaningsField, new AttributeValue
                            {
                                L = tla.AlternativeMeanings.Select(am => new AttributeValue { S = am }).ToList()
                            });
                        }

                        if (!string.IsNullOrEmpty(tla.GetAbsoluteUri()))
                        {
                            tlaMap.Add(UrlField, new AttributeValue { S = tla.GetAbsoluteUri() });
                        }

                        return new AttributeValue { M = tlaMap };
                    }).ToList()
                }
            }
        };

        return map;
    }
}