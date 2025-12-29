using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FuFood.Models.Interfaces;

namespace FuFood.Models.Entities;

public sealed class Refrigerator : IHasTimestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(10)] public required string Name { get; set; }

    public required Guid OwnerId { get; set; }
    [JsonIgnore] public User? Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime NameUpdatedAt { get; set; } = DateTime.MinValue;

    public bool IsDefault { get; set; }

    [JsonIgnore] public ICollection<RefrigeratorMembership> Memberships { get; set; } = null!;
    [JsonIgnore] public ICollection<User> Members { get; set; } = null!;
}