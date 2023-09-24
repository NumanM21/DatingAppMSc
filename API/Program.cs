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

var connectionString = "";
if (builder.Environment.IsDevelopment()) 

// What we had before in applicationserviceextensions.cs
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
else // production
{
// Use connection string provided at runtime by Fly.io
        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        // Parse connection URL to connection string for Npgsql
        connUrl = connUrl.Replace("postgres://", string.Empty);
        var pgUserPass = connUrl.Split("@")[0];
        var pgHostPortDb = connUrl.Split("@")[1];
        var pgHostPort = pgHostPortDb.Split("/")[0];
        var pgDb = pgHostPortDb.Split("/")[1];
        var pgUser = pgUserPass.Split(":")[0];
        var pgPass = pgUserPass.Split(":")[1];
        var pgHost = pgHostPort.Split(":")[0];
        var pgPort = pgHostPort.Split(":")[1];

        connectionString = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
}



builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseNpgsql(connectionString);
});

var app = builder.Build();



// Configure the HTTP request pipeline. 
// Exception handling has to go at the TOP of HTTP request pipeline

app.UseMiddleware<ExceptionMiddleware>();

// Allows any origin to access our API 
app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials() //need to allow credentials for SignalR
.WithOrigins("https://localhost:4200", "http://localhost:8080", "https://mscdatingapp.fly.dev"));

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


  await Seed.ConnectionsCleared(context);

  // Everytime we start api, this will re-seed and re-create our DB (we just have to drop DB if want to change something)

  await Seed.SeedUsers(managerRoles, appUserManager);

  var conectString = builder.Configuration.GetConnectionString("DefaultConnection");
  Console.WriteLine("Connection String: " + conectString);



}
catch (Exception e)
{
  var log = services.GetService<ILogger<Program>>();
  log.LogError(e, "Migration caused an Error");
}

app.Run();
