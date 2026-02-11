using BuildingBlocks.Infra.ReadModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.Infra.MongoReadModels;

public class PaymentReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid MongoId { get; init; }
    [BsonElement("id")]
    public int Id { get; init; }
    [BsonElement("orderId")]
    public int OrderId { get; init; }
    [BsonElement("status")]
    public string? Status { get; init; }
}
