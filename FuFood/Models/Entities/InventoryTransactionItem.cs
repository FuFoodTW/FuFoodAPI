using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FuFood.Models.Entities;

public class InventoryTransactionItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public decimal Quantity { get; set; } = 1;
    public DateOnly ExpirationDate { get; set; }
    [StringLength(255)] public string? InventoryTransactionItemImage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ParentId { get; set; } // 若消耗 product 這個會指原本入庫的 item
    public virtual InventoryTransactionItem? Parent { get; set; }

    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public Guid InventoryTransactionId { get; set; }
    [JsonIgnore] public virtual InventoryTransaction? InventoryTransaction { get; set; }
}