using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FuFood.Models.Interfaces;

namespace FuFood.Models.Entities;

public class RefrigeratorInvitation : IHasTimestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public byte[] TokenHash { get; set; } = null!;
    public int ViewCount { get; set; } = 0;

    public Guid RefrigeratorId { get; set; }
    public Refrigerator? Refrigerator { get; set; }

    public Guid CreatorId { get; set; }
    public User? Creator { get; set; }

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(1);

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}