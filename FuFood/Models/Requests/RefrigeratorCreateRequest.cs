using System.ComponentModel.DataAnnotations;

namespace FuFood.Models.Requests;

public class RefrigeratorCreateRequest
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
}