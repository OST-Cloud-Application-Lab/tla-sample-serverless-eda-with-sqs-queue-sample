using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class DynamoDbTLAReportRepository : ITLAReportRepository
{
    private readonly IAmazonDynamoDB _client = new AmazonDynamoDBClient();

    private static readonly string TableName = Environment.GetEnvironmentVariable("TLA_REPORT_TABLE_NAME")!;
    private static readonly string IdField = "id";

    public async Task<TLAReport?> FindByIdAsync(string id)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { IdField, new AttributeValue { S = id } }
            }
        };
        var response = await _client.GetItemAsync(request);
        if (response.Item != null && response.Item.Count > 0)
        {
            return TLAReportMapper.TlaReportFromDynamoDb(response.Item);
        }
        return null;
    }

    public async Task<TLAReport> SaveAsync(TLAReport report)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = TLAReportMapper.TlaReportToDynamoDb(report)
        };
        await _client.PutItemAsync(request);
        return report;
    }
}
