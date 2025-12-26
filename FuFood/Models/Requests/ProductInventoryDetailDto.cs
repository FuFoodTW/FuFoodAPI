using FuFood.Models.Enums;

namespace FuFood.Models.Requests;

public class ProductInventoryDetailDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = "";
    public UnitType Unit { get; set; }
    public ProductCategory Categories { get; set; }
    public List<InventoryBatchDto> Batches { get; set; } = [];
}