using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
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
    private readonly IMapper _autoMapper;
    public AccountController(DataContext context, ITokenService tokenService, IMapper autoMapper)
    // Classes/services we have injected using our controller's constructor
    {
      _autoMapper = autoMapper;
      _tokenService = tokenService;
      _context = context;
    }

    [HttpPost("register")] // POST Request: api/account/register (first part of class name)


    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      // Check to see if user exists
      if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
      // Go to App user from register DTO
      var user = _autoMapper.Map<AppUser>(registerDto);

      // Updating these objects with values from our register DTO
      user.UserName = registerDto.Username.ToLower();


      // pass comes to us as string from user, we have to hash it to then store it!

      // Adding user to our DB
      _context.Users.Add(user);
      await _context.SaveChangesAsync(); // Saving changes

      // What we return when a user registers
      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoURL = user.Photos.FirstOrDefault(x => x.IsMainPhoto)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.UserGender
      };
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      // Not going to repo (so don't have access to photos directly with user) -> photo is related entity (EF doesn't load these by default)
      var user = await _context.Users.Include(p => p.Photos)
      .SingleOrDefaultAsync(x =>
      x.UserName == loginDto.Username);

      if (user == null) return Unauthorized("Invalid username");

      // What we return when user logs in
      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoURL = user.Photos.FirstOrDefault(x => x.IsMainPhoto)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.UserGender
      };
    }

    private async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(AppUser => AppUser.UserName == username.ToLower());
    }
  }
}