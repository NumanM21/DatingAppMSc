
using System.Security.Claims;
// Method extension of the ClaimPrinciple --> We use this to get user!
namespace API.Extensions
{
    public static class ClaimPrincipleExtension
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
          return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }
    }
}