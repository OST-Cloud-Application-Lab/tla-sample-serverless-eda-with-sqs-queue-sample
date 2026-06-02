using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class DynamoDbTLARepository
{
    private readonly IAmazonDynamoDB _client = new AmazonDynamoDBClient();

    private static readonly string TableName = Environment.GetEnvironmentVariable("TLA_TABLE_NAME")!;
    private static readonly string NameField = "name";

    public async Task<TLAGroup?> FindByIdAsync(string name)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { NameField, new AttributeValue { S = name } }
            }
        };

        var response = await _client.GetItemAsync(request);
        if (response.Item != null && response.Item.Count > 0)
        {
            return TLAGroupMapper.TlaGroupFromDynamoDb(response.Item);
        }

        return null;
    }

    public async Task<List<TLAGroup>> FindAllAsync()
    {
        var request = new ScanRequest
        {
            TableName = TableName
        };

        var response = await _client.ScanAsync(request);
        return response.Items.Select(TLAGroupMapper.TlaGroupFromDynamoDb).ToList();
    }

    public async Task PutTlaGroupAsync(TLAGroup group)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = TLAGroupMapper.TlaGroupToDynamoDb(group),
        };
        await _client.PutItemAsync(request);
    }
}