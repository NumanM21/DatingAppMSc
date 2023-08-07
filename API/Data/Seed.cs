using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;

namespace API.Data
{
  public class Seed
  {
    public static async Task SeedUsers(DataContext context)
    {
      //FIXME: #1 Debugging
      var usersExist = await context.Users.AnyAsync();
      Console.WriteLine($"Users exist: {usersExist}");
      if (usersExist) return;

      // check to see if we have users in DB
      if (await context.Users.AnyAsync()) return;

      var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

      // change from json to c#
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

      //FIXME: #1 Debugging (Can remove later)
      foreach (var user in users)
      {
        foreach (var photo in user.Photos)
        {
          Console.WriteLine($"IsMainPhoto: {photo.IsMainPhoto}");
        }
      }

      // generate password for each user

      foreach (var user in users)
      {
        using var hmac = new HMACSHA512();

        // Handle users
        user.UserName = user.UserName.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
        user.PasswordSalt = hmac.Key;

        // Handle photos
        if (user.Photos != null)
        {
          foreach (var photo in user.Photos)
          {
            // Assign photo's AppUserId to the user's Id
            photo.AppUserId = user.Id;
            // Add photo to the context
            context.Photos.Add(photo);

          }
        }

        context.Users.Add(user);

      }

      await context.SaveChangesAsync();

    }

    //TODO: Passwords are all at default Pa$$w0rd

  }
}