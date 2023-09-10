
using System.Security.Claims;
// Method extension of the ClaimPrinciple --> We use this to get user!
namespace API.Extensions
{
  public static class ClaimPrincipleExtension
  {

    // can use Name since we set UniqueName in TokenService.cs
    public static string GetUsername(this ClaimsPrincipal user)
    {

      return user.FindFirst(ClaimTypes.Name)?.Value;
    }


    // gets user id from token
    public static int GetUserId(this ClaimsPrincipal user)
    {

      return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

  }
}