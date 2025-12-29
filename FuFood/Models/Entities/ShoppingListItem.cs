using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FuFood.Models.Enums;

namespace FuFood.Models.Entities;

public class ShoppingListItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(20)] public required string Name { get; set; }

    public decimal Quantity { get; set; } = 1;

    public UnitType Unit { get; set; }

    [StringLength(255)] public string? PhotoPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public required Guid CreatorId { get; set; }
    public User? User { get; set; }

    public required Guid ShoppingListId { get; set; }
    [JsonIgnore] public ShoppingList? ShoppingList { get; set; }
}