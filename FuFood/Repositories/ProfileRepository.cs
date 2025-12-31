using FuFood.Data;
using FuFood.Models.Requests;
using FuFood.Models.Entities;
using FuFood.Models.Enums;

namespace FuFood.Repositories;

public class ProfileRepository(AppDbContext dbContext)
{
    public async Task<User> UpdateProfile(User user, UpdateProfileRequest updateProfileRequest)
    {
        user.Name = updateProfileRequest.Name;
        if (!string.IsNullOrEmpty(updateProfileRequest.ProfilePictureUrl))
        {
            user.ProfilePictureUrl = updateProfileRequest.ProfilePictureUrl;
        }

        if (!string.IsNullOrEmpty(updateProfileRequest.Email))
        {
            user.Email = updateProfileRequest.Email;
        }

        if (updateProfileRequest.Preferences != null)
        {
            user.Preferences = updateProfileRequest.Preferences;
        }

        user.Gender = updateProfileRequest.Gender;
        user.CustomGender = user.Gender == Gender.Other ? updateProfileRequest.CustomGender : null;

        await dbContext.SaveChangesAsync();

        return user;
    }
}