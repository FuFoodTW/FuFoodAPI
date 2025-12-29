using System.ComponentModel.DataAnnotations;

namespace FuFood.Models.Requests;

public class CreateShoppingListRequest
{
    public string Title { get; set; } = null!;

    public string? CoverPhotoPath { get; set; }

    public required DateTime StartsAt { get; set; }

    public bool EnableNotifications { get; set; }
}