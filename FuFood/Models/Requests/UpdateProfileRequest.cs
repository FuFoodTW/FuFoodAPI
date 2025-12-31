namespace FuFood.Models.Requests;

public class UpdateProfileRequest
{
    public required string Name { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public string? Avatar { get; set; }

    public string? Email { get; set; }

    public List<string>? Preferences { get; set; }

    public Enums.Gender Gender { get; set; } = Enums.Gender.NotSpecified;
    public string? CustomGender { get; set; } // 性別自填欄位
}