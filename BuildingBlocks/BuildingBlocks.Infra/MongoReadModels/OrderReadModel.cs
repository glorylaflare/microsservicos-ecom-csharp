using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.Infra.MongoReadModels;

public class OrderReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid MongoId { get; init; }
    [BsonElement("userId")]
    public required string UserId { get; init; }
    [BsonElement("orderId")]
    public required int Id { get; init; }
    [BsonElement("items")]
    public required List<OrderItemReadModel> Items { get; set; } = new();
    [BsonElement("status")]
    public string Status { get; init; } = default!;
    [BsonElement("totalAmount")]
    public decimal TotalAmount { get; init; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; init; }
    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; init; }
}