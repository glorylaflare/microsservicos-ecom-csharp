using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.Infra.MongoReadModels;

public class OrderItemReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid MongoId { get; init; } = Guid.NewGuid();
    [BsonElement("productId")]
    public int ProductId { get; init; }
    [BsonElement("quantity")]
    public int Quantity { get; init; }
}