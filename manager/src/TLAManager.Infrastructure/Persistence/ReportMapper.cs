using Amazon.DynamoDBv2.Model;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public static class ReportMapper
{
    private static readonly string IdField = "id";
    private static readonly string StatusField = "status";
    private static readonly string UrlField = "url";

    public static Report ReportFromDynamoDb(Dictionary<string, AttributeValue> items)
    {
        var builder = new Report
        {
            Id = items[IdField].S,
            Status = Enum.Parse<ReportStatus>(items[StatusField].S, ignoreCase: true),
            Url = items[UrlField].S
        };

        return builder;
    }

    public static Dictionary<string, AttributeValue> ReportToDynamoDb(Report report)
    {
        var map = new Dictionary<string, AttributeValue>
        {
            { IdField, new AttributeValue { S = report.Id } },
            { StatusField, new AttributeValue { S = report.Status.ToString() } },
            { UrlField, new AttributeValue { S = report.Url } }
        };

        return map;
    }
}