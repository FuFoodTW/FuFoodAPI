using FuFood.Data;
using FuFood.Models.Requests;
using FuFood.Models.Entities;

namespace FuFood.Repositories;

public class ProfileRepository(AppDbContext dbContext)
{
    public async Task<User> UpdateProfile(User user, UpsertProfileRequest upsertProfileRequest)
    {
        user.Name = upsertProfileRequest.Name;
        user.ProfilePictureUrl = upsertProfileRequest.ProfilePictureUrl;
        user.Email = upsertProfileRequest.Email;
        user.Preference = upsertProfileRequest.Preference;
        user.Gender = upsertProfileRequest.Gender;
        user.CustomGender = upsertProfileRequest.CustomGender;

        await dbContext.SaveChangesAsync();

        return user;
    }
}