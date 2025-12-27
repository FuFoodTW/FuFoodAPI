using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuFood.Models.Entities;

public class RefrigeratorMember
{
    public Guid RefrigeratorId { get; set; }
    public virtual Refrigerator? Refrigerator { get; set; }

    public Guid MemberId { get; set; }
    public virtual User? Member { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
