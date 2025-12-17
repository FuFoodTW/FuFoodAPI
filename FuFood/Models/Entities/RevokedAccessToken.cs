using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models;

[Index(nameof(TokenHash), IsUnique = true)]
[Index(nameof(ExpiresAt))]
public class RevokedAccessToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required byte[] TokenHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}