using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
  public class TokenService : ITokenService
    {
      private readonly SymmetricSecurityKey _key;
      public TokenService(IConfiguration config)
      {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
      }
      public string CreateToken(AppUser user)
      {
        var claims = new List<Claim> // claim is info user 'claims'
        {
          new Claim(JwtRegisteredClaimNames.NameId, user.UserName) 
          // we set the claim to userName
        };

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