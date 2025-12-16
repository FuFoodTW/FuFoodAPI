using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models;

[Index(nameof(LineId), IsUnique = true)]
public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(255)] public required string LineId { get; set; }

    [StringLength(255)] public required string Name { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}