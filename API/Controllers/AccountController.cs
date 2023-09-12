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
    private readonly UserManager<AppUser> _appUserManager;

    private readonly ITokenService _tokenService;
    private readonly IMapper _autoMapper;
    public AccountController(UserManager<AppUser> appUserManager, ITokenService tokenService, IMapper autoMapper)
    // Classes/services we have injected using our controller's constructor
    {
      _appUserManager = appUserManager;
      _autoMapper = autoMapper;
      _tokenService = tokenService;

    }

    // POST Request: api/account/register (first part of class name)

    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      // Check to see if user exists
      if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

      // Go to App user from register DTO
      var user = _autoMapper.Map<AppUser>(registerDto);

      // Updating these objects with values from our register DTO
      user.UserName = registerDto.Username.ToLower();

      // Adding user to our DB
      var res = await _appUserManager.CreateAsync(user, registerDto.Password);

      // if user creation fails
      if (res.Succeeded == false) return BadRequest(res.Errors);

      // Member registered at this point, so add to member role(default)
      var resultRole = await _appUserManager.AddToRoleAsync(user, "Member");

      // if role creation fails
      if (resultRole.Succeeded == false) return BadRequest(resultRole.Errors);

      // What we return when a user registers
      return new UserDto
      {
        Username = user.UserName,
        Token = await _tokenService.CreateToken(user),
        PhotoURL = user.Photos.FirstOrDefault(x => x.IsMainPhoto)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.UserGender
      };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      // Not going to repo (so don't have access to photos directly with user) -> photo is related entity (EF doesn't load these by default)
      var user = await _appUserManager.Users.Include(p => p.Photos)
      .SingleOrDefaultAsync(x =>
      x.UserName == loginDto.Username);


      // Check user exists 
      if (user == null) return Unauthorized("Username is invalid. Please check your username and try again.");

      // Check password is correct (ASP.NET Core Identity does this for us)
      var res = await _appUserManager.CheckPasswordAsync(user, loginDto.Password);

      if (res == false) return Unauthorized("Password is invalid.");


      // What we return when user logs in
      return new UserDto
      {
        Username = user.UserName,
        Token = await _tokenService.CreateToken(user),
        PhotoURL = user.Photos.FirstOrDefault(x => x.IsMainPhoto)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.UserGender
      };
    }

    private async Task<bool> UserExists(string username)
    {
      return await _appUserManager.Users.AnyAsync(AppUser => AppUser.UserName == username.ToLower());
    }
  }
}