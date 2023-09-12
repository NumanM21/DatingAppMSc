using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
  public class TokenService : ITokenService
  {
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _managerUser;
    public TokenService(IConfiguration config, UserManager<AppUser> managerUser)
    {
      _managerUser = managerUser;
      _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }
    public async Task<string> CreateToken(AppUser user)
    {
      var claims = new List<Claim> // claim is info user 'claims'
        {
          new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName.ToString()),

          new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()) 
          // we set the claim to specific id of user -> more efficient than storing all user info in token
        };

      // get role(s) for user
      var roles = await _managerUser.GetRolesAsync(user);

      // add roles to claims (only want name of role user is in -> Use select() which is LINQ projection) 
      claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r))); 
      // r is array of roles if multiple


      // signing credentials which we sign this token with 
      var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

      // describe token we return  
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(7), // token expire after 1 week 
        SigningCredentials = creds
      };

      // Token handler
      var tokenHandler = new JwtSecurityTokenHandler();

      // Create token
      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }

  }
}