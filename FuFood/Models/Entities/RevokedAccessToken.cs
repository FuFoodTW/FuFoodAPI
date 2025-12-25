using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models;

[Index(nameof(TokenHash), IsUnique = true)]
[Index(nameof(ExpiresAt))]
public class RevokedAccessToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required byte[] TokenHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}