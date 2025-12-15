using System.Text.RegularExpressions;
using FuFood.Models.Enums;

namespace FuFood.Models;

public class Inventory
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; }
    public int Quantity { get; set; }
    public UnitType Unit { get; set; } // 單位
    public string? ImagePath { get; set; }
    public InventoryStatus Status { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool LowStockAlert { get; set; } // 是否開啟低庫存警示
    public int LowStockThreshold { get; set; } // 低庫存警示閥值
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 使用者
    public Guid UserId { get; set; }
    public User User { get; set; }

    // 群組可有可無
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
}