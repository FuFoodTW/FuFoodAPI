using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FuFood.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Models.Entities;

[Index(nameof(RefrigeratorId), nameof(MemberId), IsUnique = true)]
public class RefrigeratorMember : IHasTimestamp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid RefrigeratorId { get; set; }
    public virtual Refrigerator? Refrigerator { get; set; }

    public Guid MemberId { get; set; }
    public virtual User? Member { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}