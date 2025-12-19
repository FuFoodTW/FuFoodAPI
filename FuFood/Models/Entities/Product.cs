using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FuFood.Models.Enums;

namespace FuFood.Models.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [StringLength(255)] public required string Name { get; set; }
    public decimal Quantity { get; set; }
    public required UnitType Unit { get; set; } // 單位
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}