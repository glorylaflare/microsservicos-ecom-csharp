using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.Infra.MongoReadModels;

public class UserReadModel
{
    [BsonId]
    [BsonElement("id")]
    public string Id { get; init; } = string.Empty;
    [BsonElement("username")]
    public string Username { get; init; } = string.Empty;
    [BsonElement("email")]
    public string Email { get; init; } = string.Empty;
}