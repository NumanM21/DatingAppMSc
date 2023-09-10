using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.ExternalHelpers
{
  // Action filter gives us access to HttpContext (to get username and services)
  public class UserActiveLogger : IAsyncActionFilter
  {
    // to log when user was last active
    // next parameter -> Can do something before or after the action is executed (before it hits our API controller)


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      // this is the action being executed (API action completed (after it hits our API controller))

      var contextRes = await next();

      // check if user is authenticated -> Safety check 
      if (!contextRes.HttpContext.User.Identity.IsAuthenticated) return;

      // get user id from token
      var userID = contextRes.HttpContext.User.GetUserId();

      // get user from repo -> to update last active property
      var repository = contextRes.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
      var user = await repository.AsyncGetUserByID(userID);
      user.LastActive = DateTime.UtcNow; // set to current time
      await repository.AsyncSaveAll();

    }
  }
}