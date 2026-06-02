using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TLAManager.Application.Interfaces;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class DynamoDbReportRepository : IReportRepository
{
    private readonly IAmazonDynamoDB _client = new AmazonDynamoDBClient();

    private static readonly string TableName = Environment.GetEnvironmentVariable("TLA_REPORT_TABLE_NAME")
        ?? throw new InvalidOperationException("TLA_REPORT_TABLE_NAME is not configured");
    private static readonly string IdField = "id";

    public async Task<Report?> FindByIdAsync(string id)
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
            return ReportMapper.ReportFromDynamoDb(response.Item);
        }
        return null;
    }

    public async Task<Report> SaveAsync(Report report)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = ReportMapper.ReportToDynamoDb(report)
        };
        await _client.PutItemAsync(request);
        return report;
    }
}
