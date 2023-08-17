using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// These are all endpoints 

namespace API.Controllers
{
  [Authorize] // Wherever we place this, need to authorize user to access all endpoints after it (our authorization is jwt token)
  public class UsersController : BaseApiController
  {


    private readonly IUserRepository _userRepository;
    private readonly IMapper _autoMapper;

    public UsersController(IUserRepository userRepository, IMapper autoMapper)
    {
      _autoMapper = autoMapper;
      _userRepository = userRepository;



    }


    [HttpGet] // Api end point (this + route make up our API root)
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() // gets all users
    {
      var users = await _userRepository.AsyncGetMembers();

      return Ok(users);

    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username) // get individual user
    {
      return await _userRepository.AsyncGetMember(username);
    }

    [HttpPut]
    public async Task<ActionResult> UserUpdate(UpdateMemberDto updateMemberDto)
    {
      // User also authenticated, so can get their username from their token
      var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

      var user = await _userRepository.AsyncGetUserByUsername(username);

      // For safety measure
      if (user == null) return NotFound();

      // Updates all properties passed from updateMemberDto, and over-writes these properties in the user (EF tracks all changes to user automatically here)
      _autoMapper.Map(updateMemberDto, user);

      // Now we have to SAVE the changes to the data for THAT user
      if (await _userRepository.AsyncSaveAll()) return NoContent();

      // Nothing saved to DB (No changes)
      return BadRequest("User not updated");
    }

  }
}
