using System.ComponentModel.DataAnnotations;

namespace FuFood.Models.Requests;

public class RefrigeratorUpdateRequest
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
}