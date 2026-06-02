using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TLAManager.Domain;

namespace TLAManager.Infrastructure.Persistence;

public class DynamoDbTLARepository
{
    private readonly IAmazonDynamoDB _client = new AmazonDynamoDBClient();

    private static readonly string TableName = Environment.GetEnvironmentVariable("TLA_TABLE_NAME")
        ?? throw new InvalidOperationException("TLA_TABLE_NAME is not configured");
    private static readonly string NameField = "name";

    public async Task<Group?> FindByIdAsync(string name)
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
            return GroupMapper.GroupFromDynamoDb(response.Item);
        }

        return null;
    }

    public async Task<List<Group>> FindAllAsync()
    {
        var request = new ScanRequest
        {
            TableName = TableName
        };

        var response = await _client.ScanAsync(request);
        return response.Items.Select(GroupMapper.GroupFromDynamoDb).ToList();
    }

    public async Task PutTlaGroupAsync(Group group)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = GroupMapper.GroupToDynamoDb(group),
        };
        await _client.PutItemAsync(request);
    }
}