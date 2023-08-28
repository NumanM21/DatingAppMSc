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
      var username = User.GetUsername(); // This method from ClaimPrincipleExtension(Can create extension methods for pre-made classes and use them!)

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

    [HttpPost("add-photo")] 
    // adding a route parameter to our HttpPost we already have (This will allow users to upload picture)
    // Should get a HTTP 201 response 

    public async Task<ActionResult<PhotoDto>> photoAdd (IFormFile file)
    {
      var username = User.GetUsername();
      // Since we use our repo here, EF auto tracks the user
      var user = await _userRepository.AsyncGetUserByUsername(username);

      if (user == null) return NotFound(); // check in-case we don't have user 
      
        var imgUpload = await _servicePhoto.AsyncAddPhoto(file);
        // Can absolute uri is stored in our DB (we use this to track the image in cloudinary)
        
        // Check if we  have error with imgUpload
        if (imgUpload.Error != null) return BadRequest(imgUpload.Error.Message); 

        var img = new Photo
        {
          Url = imgUpload.SecureUrl.AbsoluteUri,
          PublicId = imgUpload.PublicId
        };

        // If first photo, have to set this to main 
        if (user.Photos.Count == 0) img.IsMainPhoto = true;
        
        //Now the EF will track this user in memory
        user.Photos.Add(img);

        if(await _userRepository.AsyncSaveAll()) return _autoMapper.Map<PhotoDto>(img);

        return BadRequest("Photo not mapped to DTO");

      

    }

  }
}
