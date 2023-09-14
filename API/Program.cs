using System.Diagnostics.CodeAnalysis;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();



// Configure the HTTP request pipeline. 
// Exception handling has to go at the TOP of HTTP request pipeline

app.UseMiddleware<ExceptionMiddleware>();

// Allows any origin to access our API 
app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials() //need to allow credentials for SignalR
.WithOrigins("https://localhost:4200"));

// Middleware comes between the .UseCors and .MapController

app.UseAuthentication(); // Checks for valid token
app.UseAuthorization(); // Sees if this valid token is what we're looking for!

// endpoints for SignalR
app.MapHub<UserPresenceHub>("hubs/user-presence");
app.MapHub<UserMessageHub>("hubs/users-message");

app.MapControllers();

// How we actually seed data into our DB
using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try
{

  var context = services.GetRequiredService<DataContext>();

  var appUserManager = services.GetRequiredService<UserManager<AppUser>>();
   var managerRoles = services.GetRequiredService<RoleManager<Roles>>();
  await context.Database.MigrateAsync();

  // Everytime we start api, this will re-seed and re-create our DB (we just have to drop DB if want to change something)

  await Seed.SeedUsers(managerRoles, appUserManager);

}
catch (Exception e)
{
  var log = services.GetService<ILogger<Program>>();
  log.LogError(e, "Migration caused an Error");
}

app.Run();
