namespace FuFood.Models.Requests;

public class InventoryTransactionItemCreateRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public string? Image { get; set; }
}