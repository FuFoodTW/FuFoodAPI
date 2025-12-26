namespace FuFood.Models.Requests;

public class InventoryConsumeRequest
{
    public Guid InventoryItemId { get; set; } // 要消耗哪一批 = Parent item 的 Id
    public decimal Quantity { get; set; }
}