using System.ComponentModel.DataAnnotations;

namespace FuFood.Models.Requests;

public class UpsertShoppingListRequest
{
    [StringLength(10)] public required string Title { get; set; }

    public string? CoverPhotoPath { get; set; }

    public required DateTime StartsAt { get; set; }

    public bool EnableNotifications { get; set; }
}