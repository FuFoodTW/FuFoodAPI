using System.ComponentModel.DataAnnotations;
using FuFood.Models.Enums;

namespace FuFood.Models.Requests;

public class CreateShoppingListItemRequest
{
    public required string Name { get; set; } = null!;

    public decimal Quantity { get; set; } = 1;

    public UnitType Unit { get; set; }

    public string? PhotoPath { get; set; }
}