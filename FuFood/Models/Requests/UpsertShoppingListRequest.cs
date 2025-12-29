using System.ComponentModel.DataAnnotations;

namespace FuFood.Models.Requests;

public class UpsertShoppingListRequest
{
    public required string Title { get; set; }

    public string? CoverPhotoPath { get; set; }

    public required DateTime StartsAt { get; set; }

    public bool EnableNotifications { get; set; }
}