namespace FuFood.Models.Entities;

public class InventoryTransaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<InventoryTransactionItem> Items { get; set; } = []; // 一對多關聯

    public required Guid RefrigeratorId { get; set; }
    public virtual Refrigerator? Refrigerator { get; set; }

    public required Guid UserId { get; set; } // FK
    public virtual User? User { get; set; } // 關聯
}