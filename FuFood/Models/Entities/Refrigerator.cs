using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models.Entities;

public class Refrigerator
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [StringLength(100)] public required string Name { get; set; }
    [StringLength(30)] public string? Colour { get; set; }

    public required Guid CreatedById { get; set; }
    public virtual User? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDefault { get; set; } = false;
}