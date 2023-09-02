using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
    private readonly IPhotoService _servicePhoto;

    public UsersController(IUserRepository userRepository, IMapper autoMapper, IPhotoService servicePhoto)
    {
      _servicePhoto = servicePhoto;
      _autoMapper = autoMapper;
      _userRepository = userRepository;



    }


    [HttpGet] // Api end point (this + route make up our API root)
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() // gets all users
    {
      var users = await _userRepository.AsyncGetMembers();

      return Ok(users); // HTTP 200 

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
      var username = User.GetUsername(); // This method from ClaimPrincipleExtension(Can create extension methods for pre-made classes and use them!)

      var user = await _userRepository.AsyncGetUserByUsername(username);

      // For safety measure
      if (user == null) return NotFound();

      // Updates all properties passed from updateMemberDto, and over-writes these properties in the user (EF tracks all changes to user automatically here)
      _autoMapper.Map(updateMemberDto, user);

      // Now we have to SAVE the changes to the data for THAT user
      if (await _userRepository.AsyncSaveAll()) return NoContent(); // HTTP 204

      // Nothing saved to DB (No changes)
      return BadRequest("User not updated");
    }

    [HttpPost("add-photo")]
    // adding a route parameter to our HttpPost we already have (This will allow users to upload picture)
    // Should get a HTTP 201 created response

    //TODO: 1. Separate this larger method into smaller methods -> can have logic of setting main Photo as a service layer or domain logic. 2. More specific error handling, have console logs for specific issues (easier to debug). 
    public async Task<ActionResult<PhotoDto>> photoAdd(IFormFile file)
    {
      var username = User.GetUsername();
      // Since we use our repo here, EF auto tracks the user
      var user = await _userRepository.AsyncGetUserByUsername(username);

      if (user == null) return NotFound("User not found in photoAdd method!"); // check in-case we don't have user (HTTP 404 Err)

      var imgUpload = await _servicePhoto.AsyncAddPhoto(file);
      // Can absolute uri is stored in our DB (we use this to track the image in cloudinary)

      // Check if we  have error with imgUpload
      if (imgUpload.Error != null) return BadRequest(imgUpload.Error.Message); //HHTP 400 Err

      var img = new Photo
      {
        Url = imgUpload.SecureUrl.AbsoluteUri,
        PublicId = imgUpload.PublicId
      };

      // If first photo, have to set this to main 
      if (user.Photos.Count == 0) img.IsMainPhoto = true;

      //Now the EF will track this user in memory
      user.Photos.Add(img);

      if (await _userRepository.AsyncSaveAll())
      {
        return CreatedAtAction(
        nameof(GetUser), // Sending back a location header for client to get img created, they go through username endpoint (api/users/username)
        new { username = user.UserName }, // new object, assign username of user to username variable -> This is the argument we pass to GetUser
        _autoMapper.Map<PhotoDto>(img));  // Pass object we have created back ->passing back photoDto, and we map FROM the img we created 
      }


      return BadRequest("Photo not mapped to DTO"); // HTTP 400 Err



    }

    [HttpPut("set-photo-main/{photoId}")] // Put for updating resource (selecting Main photo)
    public async Task<ActionResult> SetPhotoMain(int photoId) // EF Tracks these changes automatically --> So have to update DB at the end
    {
      var user = await _userRepository.AsyncGetUserByUsername(User.GetUsername());

      if (user == null) return NotFound("User not found, is null");

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); // can return null

      if (photo == null) return NotFound("id to match Photo not found");

      if (photo.IsMainPhoto) return BadRequest("Photo already set to Main photo"); 
      // In-case user uses 3rd party tool to update photo (app safety)

      var currMain = user.Photos.FirstOrDefault(x => x.IsMainPhoto);
      // Already have a main photo, we disable it / remove it via the bool flag
      if (currMain != null) currMain.IsMainPhoto = false;

      photo.IsMainPhoto = true;

      if (await _userRepository.AsyncSaveAll()) return NoContent(); // update
      // Failed to save all if we return below
      return BadRequest("Main photo couldn't be updated!");
    }

  }
}
