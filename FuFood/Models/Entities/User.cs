using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FuFood.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models.Entities;

[Index(nameof(LineId), IsUnique = true)]
public class User : IHasTimestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(255)] public required string LineId { get; set; }

    [StringLength(255)] public required string Name { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public FuFood.Models.Enums.SubscriptionTier SubscriptionTier { get; set; } =
        FuFood.Models.Enums.SubscriptionTier.Free;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}