using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();



// Configure the HTTP request pipeline. 
// Exception handling has to go at the TOP of HTTP request pipeline

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("https://localhost:4200"));

// Middleware comes between the .UseCors and .MapController

app.UseAuthentication(); // Checks for valid token
app.UseAuthorization(); // Sees if this valid token is what we're looking for!

app.MapControllers();

app.Run();
