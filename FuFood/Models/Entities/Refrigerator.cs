using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FuFood.Models.Interfaces;

namespace FuFood.Models.Entities;

public class Refrigerator : IHasTimestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(10)] public required string Name { get; set; }
    [StringLength(30)] public string? Colour { get; set; }

    public required Guid OwnerId { get; set; }
    public virtual User? Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime NameUpdatedAt { get; set; } = DateTime.MinValue;

    public bool IsDefault { get; set; } = false;

    public virtual ICollection<RefrigeratorMember> Members { get; set; } = new List<RefrigeratorMember>();
}