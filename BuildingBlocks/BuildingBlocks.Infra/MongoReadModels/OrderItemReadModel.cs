using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.Infra.MongoReadModels;

public class OrderItemReadModel
{
    [BsonId]
    [BsonElement("productId")]
    public int ProductId { get; init; }
    [BsonElement("quantity")]
    public int Quantity { get; init; }
}