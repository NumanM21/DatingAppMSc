using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
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

app.UseDefaultFiles(); // looks for index.html in wwwroot folder (index default if file not found) 
app.UseStaticFiles(); // looks for static files in wwwroot folder (css, js, etc)

// endpoints for SignalR
app.MapHub<UserPresenceHub>("hubs/user-presence");
app.MapHub<UserMessageHub>("hubs/users-message");

app.MapControllers();


app.MapFallbackToController("Index", "FallBack"); // if we refresh and we are on a page that doesn't exist


// How we actually seed data into our DB
using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try
{

  var context = services.GetRequiredService<DataContext>();

  var appUserManager = services.GetRequiredService<UserManager<AppUser>>();
   var managerRoles = services.GetRequiredService<RoleManager<Roles>>();
  await context.Database.MigrateAsync();

  // clear connection in DB -> when we start api, we want to clear all connections --> Issue in larger DB's
  // context.GroupConnection.RemoveRange(context.GroupConnection);

  // Instead have SQL Query to clear all connections (truncate table)
  await context.Database.ExecuteSqlRawAsync("DELETE FROM [GroupConnection]"); // truncate table is faster than remove range --> DELETE FROM is sqlLite syntax!

  // Everytime we start api, this will re-seed and re-create our DB (we just have to drop DB if want to change something)

  await Seed.SeedUsers(managerRoles, appUserManager);

}
catch (Exception e)
{
  var log = services.GetService<ILogger<Program>>();
  log.LogError(e, "Migration caused an Error");
}

app.Run();
