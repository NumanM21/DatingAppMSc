using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;

namespace API.Data
{
  public class Seed
  {
    public static async Task SeedUsers(UserManager<AppUser> appUserManager)
    {  

      // check to see if we have users in DB
      if (await appUserManager.Users.AnyAsync()) return;

      var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

      // change from json to c#
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

     
      // generate password for each user

      foreach (var user in users)
      {
        // Handle users
        user.UserName = user.UserName.ToLower();

        await appUserManager.CreateAsync(user, "Pa$$w0rd");  
        // Creates user and hashes password and save to DB

      }

    }

    //TODO: Passwords are all at default Pa$$w0rd -> REMOVE THIS!(Before submitting project)

  }
}