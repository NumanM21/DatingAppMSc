using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SQLitePCL;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
      _tokenService = tokenService;
      _context = context;
    }

    [HttpPost("register")] // POST Request: api/account/register (first part of class name)


    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

      using var hmac = new HMACSHA512(); // key from HMAC is used as our passwordSalt

      var user = new AppUser
      {
        UserName = registerDto.Username.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        PasswordSalt = hmac.Key
        // pass comes to us as string from user, we have to hash it to then store it!
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoURL = user.Photos.FirstOrDefault(x => x.IsMainPhoto)?.Url
      };
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      // Not going to repo (so don't have access to photos directly with user) -> photo is related entity (EF doesn't load these by default)
      var user = await _context.Users.Include(p=>p.Photos)
      .SingleOrDefaultAsync(x =>
      x.UserName == loginDto.Username);

      if (user == null) return Unauthorized("Invalid username");

      using var hmac = new HMACSHA512(user.PasswordSalt); // if same pass, same hashing

      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (int i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
      }

      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoURL = user.Photos.FirstOrDefault(x=>x.IsMainPhoto)?.Url
      };
    }

    private async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(AppUser => AppUser.UserName == username.ToLower());
    }
  }
}