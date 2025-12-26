namespace FuFood.Models.Requests;

public class InventoryBatchDto
{
    public Guid ItemId { get; set; }
    public decimal RemainingQuantity { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
}