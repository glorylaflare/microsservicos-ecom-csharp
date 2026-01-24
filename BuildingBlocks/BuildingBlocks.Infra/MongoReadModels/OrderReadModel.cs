using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.Infra.MongoReadModels;

public class OrderReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid MongoId { get; init; } = Guid.NewGuid();
    [BsonElement("orderId")]
    public int Id { get; init; }
    [BsonElement("items")]
    public List<OrderItemReadModel> Items { get; set; } = new();
    [BsonElement("status")]
    public string Status { get; init; } = string.Empty;
    [BsonElement("totalAmount")]
    public decimal TotalAmount { get; init; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; init; }
    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; init; }
}