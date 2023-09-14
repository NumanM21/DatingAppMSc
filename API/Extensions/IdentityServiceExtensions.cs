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

      .AddRoleManager<RoleManager<Roles>>() 
      // add role manager to identity

      .AddEntityFrameworkStores<DataContext>(); 
      // create all identity related tables in database

      // Authentication first -> then Authorization
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

          // Authenticate signalR 

          options.Events = new JwtBearerEvents
          {
            // When a user connects to the hub
            OnMessageReceived = cxt => {
              
              // get the access token from the query string
              var tokenToAccessHub = cxt.Request.Query["access_token"]; 
              // pre-defined query string from signalR

              var pathToHub = cxt.HttpContext.Request.Path;

              // if the user is trying to access the hub
              if(!string.IsNullOrEmpty(tokenToAccessHub) && pathToHub.StartsWithSegments("/hubs")) // hubs has to match first path of .MapHub in Program.cs
              {
                // on the right path and has a token -> set the token to the token from the query string
                cxt.Token = tokenToAccessHub;
                // hub now has access to our bearer token (which is our JWT token)
              }

              // return the context
              return Task.CompletedTask;
            }
          };
        });


      // Authorization
      services.AddAuthorization(
        options =>
      {

        options.AddPolicy("AdminRoleRequired", pol => pol.RequireRole("Admin"));

        options.AddPolicy("ModeratorRoleRequired", pol => pol.RequireRole("Moderator", 
        "Admin"));
      });

        return services;
    }
  }
}