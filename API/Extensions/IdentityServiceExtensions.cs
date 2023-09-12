using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
  public static class IdentityServiceExtensions
  {
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
      // AddIdentityCore is used when we don't want to use EntityFramework
      // Can configure stuff like user, pass, login, email and etc

      services.AddIdentityCore<AppUser>()

      .AddRoles<Roles>() 
      // add roles to identity

      .AddRoleManager<RoleManager<AppUserWithRoles>>() 
      // add role manager to identity

      .AddEntityFrameworkStores<DataContext>(); 
      // create all identity related tables in database


      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.
                UTF8.GetBytes(config["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false
          };
        });

        return services;
    }
  }
}