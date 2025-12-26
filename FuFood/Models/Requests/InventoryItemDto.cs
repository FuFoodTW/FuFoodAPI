using FuFood.Models.Enums;

namespace FuFood.Models.Requests;

public class InventoryItemDto
{
    public Guid ItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public ProductCategory Category { get; set; }
    public UnitType Unit { get; set; }
    public decimal RemainingQuantity { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
}