using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models.Entities;

[Index(nameof(CommittedAt))]
public class InventoryTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<InventoryTransactionItem> Items { get; set; } = []; // 一對多關聯

    // 交易是否草稿，如果這個時間為空那就是草稿
    public DateTime? CommittedAt { get; set; }

    public required Guid RefrigeratorId { get; set; }
    public virtual Refrigerator? Refrigerator { get; set; }

    public required Guid UserId { get; set; } // FK
    public virtual User? User { get; set; } // 關聯
}