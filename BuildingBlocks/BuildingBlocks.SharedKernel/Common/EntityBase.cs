using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.SharedKernel.Common;

public class EntityBase
{
    [Key]
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
