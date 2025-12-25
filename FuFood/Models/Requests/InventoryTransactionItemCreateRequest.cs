using FuFood.Models.Enums;

namespace FuFood.Models.Requests;

public class ProductParams
{
    public required string Name { get; set; }
    public decimal Quantity { get; set; }
    public required UnitType Unit { get; set; } // 單位
    public ProductCategory Categories { get; set; } = ProductCategory.None;
}

public class InventoryTransactionItemCreateRequest
{
    public Guid? ProductId { get; set; }
    public ProductParams? ProductParams { get; set; }
    public decimal Quantity { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public string? Image { get; set; }
}