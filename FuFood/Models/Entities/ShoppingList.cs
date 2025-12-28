using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FuFood.Models.Enums;

namespace FuFood.Models.Entities;

public class ShoppingList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(10)] public required string Title { get; set; }

    public string? CoverPhotoPath { get; set; }

    public required DateTime StartsAt { get; set; }

    public bool EnableNotifications { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public required Guid RefrigeratorId { get; set; }
    public virtual Refrigerator? Refrigerator { get; set; }

    public virtual ICollection<ShoppingListItem> Items { get; set; } = [];
}