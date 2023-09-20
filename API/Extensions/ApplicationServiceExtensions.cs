using API.Data;
using API.ExternalHelpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;


namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
      // Configures the DataContext to use SQLite with the connection string named "DefaultConnection" from the configuration.
      services.AddDbContext<DataContext>(opt =>
        {
          opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
      // Cors helps with security and flexibility
      services.AddCors();
      // Scoped to our HTTP request -> This makes these classes injectable to our user controller (.AddScoped means this service is created NEW for each HTTP request -> broken after the request is complete)
      services.AddScoped<ITokenService, TokenService>();
      //** services.AddScoped<IUserRepository,UserRepository>();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      services.Configure<SettingsCloudinary>(config.GetSection("SettingsCloudinary"));
      services.AddScoped<IPhotoService, ServicePhoto>();
      //** services.AddScoped<ILikeRepository, LikeRepository>(); //Interface and then Implementation class of that service
      services.AddScoped<UserActiveLogger>();
      //** services.AddScoped<IMessageUserRepository, MessageUserRepository>(); // Logging repo into our controller -> now usingUnitOfWork to do this
      services.AddSignalR();
      // singleton -> only one instance of this class is created and used throughout the application
      services.AddSingleton<UserPresenceTracker>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<ServicePhoto>();

      // debug
      services.AddControllers().AddNewtonsoftJson(options =>
      { 
       options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      });





      return services;
    }
  }
}