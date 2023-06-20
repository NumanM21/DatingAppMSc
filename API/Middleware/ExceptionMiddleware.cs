using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace API.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _exceptionLogger;
    private readonly IHostEnvironment _environment;
    public ExceptionMiddleware(RequestDelegate next, 
    ILogger<ExceptionMiddleware> exceptionLogger, IHostEnvironment environment)
    {
      _environment = environment;
      _exceptionLogger = exceptionLogger;
      _next = next; // tells computer next bit of middleware we need to go to 
    }

// middleware goes between different middleware calling 'next'. So it needs a InvokeAsync() method to begin 
    public async Task InvokeAsync(HttpContext context) 
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _exceptionLogger.LogError(e, e.Message); // for us to see
            context.Response.ContentType = "application/json"; // return to client
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 

            var response = _environment.IsDevelopment()? 
            new ApiException(context.Response.StatusCode, e.Message, e.StackTrace?.ToString()) : new ApiException(context.Response.StatusCode, e.Message, "Internal Server Error");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
  }
}