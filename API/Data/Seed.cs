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
    public static async Task SeedUsers(RoleManager<Roles> managerRoles, UserManager<AppUser> appUserManager)
    {

      // check to see if we have users in DB
      if (await appUserManager.Users.AnyAsync()) return;

      var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

      // change from json to c#
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

      // Creating new roles for users //TODO: Can create a PREMIUM user role here if we have time
      var appRole = new List<Roles>
      {
        // in db userRole -> 1 = admin, 2 = moderator, 3 = member
        new() {Name = "Admin"},
        new() {Name = "Moderator"},
        new() {Name = "Member"},
      };

      // Add each role to DB
      foreach (var r in appRole)
      {
        await managerRoles.CreateAsync(r);
      }


      // Add each user to roles
      foreach (var user in users)
      {
        // Handle users
        user.UserName = user.UserName.ToLower();

        // Approve first photo to TRUE for seeded users
        user.Photos.First().IsPhotoApproved = true;

        // Creates user and hashes password and save to DB
        await appUserManager.CreateAsync(user, "Pa$$w0rd");

        // Add user to role => Default to member
        await appUserManager.AddToRoleAsync(user, "Member");
      }

      // Create admin user
      var adminUser = new AppUser{UserName = "admin"};
      await appUserManager.CreateAsync(adminUser, "Pa$$w0rd");

      // Add admin to admin role (can add to multiple roles)
      await appUserManager.AddToRolesAsync(adminUser, new[] {"Admin", "Moderator"});

    }


  }

  //TODO: Passwords are all at default Pa$$w0rd -> REMOVE THIS!(Before submitting project)

}
