using System.Security.Claims;
using API.Data;
using API.Data.Migrations;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.ExternalHelpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// These are all endpoints 

namespace API.Controllers
{
  // Wherever we place this, need to authorize user to access all endpoints after it (our authorization is jwt token)

  [Authorize]
  public class UsersController : BaseApiController
  {



    private readonly IMapper _autoMapper;
    private readonly IPhotoService _servicePhoto;
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork, IMapper autoMapper, IPhotoService servicePhoto)
    {
      _unitOfWork = unitOfWork;
      _servicePhoto = servicePhoto;
      _autoMapper = autoMapper;
    }



    [HttpGet]
    public async Task<ActionResult<PaginationList<MemberDto>>> GetUsers([FromQuery] ParameterFromUser parameterFromUser)
    // Ask client to sent this as query string hence we use [FromQuery] (since ParameterFromUser is a object)
    {
      //get current user's gender
      var genderOfUser = await _unitOfWork.RepositoryUser.GenderOfUser(User.GetUsername());

      // populate parameterFromUser with current user's username (from token)
      parameterFromUser.currUsername = User.GetUsername();

      // Default member page to display is opposite gender (unless specified otherwise)

      if (string.IsNullOrEmpty(parameterFromUser.gender))
      { // setting parameter to DEFAULT value
        parameterFromUser.gender = genderOfUser == "male" ? "female" : "male";
      }


      var users = await _unitOfWork.RepositoryUser.AsyncGetMembers(parameterFromUser);


      // return pagination info using pagination header
      Response.HeadPaginationAdd(new HeadPagination(users.currentPage, users.PageSize, users.totalCount, users.totalPage));

      return Ok(users); // HTTP 200 
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username) // get individual user
    {
      // get username of current user
      var currUser = User.GetUsername();

      // if currUser == username provided in client -> return true so we show user the unapprove photos -> else it remains hidden
      return await _unitOfWork.RepositoryUser.AsyncGetMember(username, currUser == username);

    }

    [HttpPut]
    public async Task<ActionResult> UserUpdate(UpdateMemberDto updateMemberDto)
    {
      // User also authenticated, so can get their username from their token
      var username = User.GetUsername(); // This method from ClaimPrincipleExtension(Can create extension methods for pre-made classes and use them!)

      var user = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(username);

      // For safety measure
      if (user == null) return NotFound();

      // Updates all properties passed from updateMemberDto, and over-writes these properties in the user (EF tracks all changes to user automatically here)
      _autoMapper.Map(updateMemberDto, user);

      // Now we have to SAVE the changes to the data for THAT user
      if (await _unitOfWork.TransactionComplete()) return NoContent(); // HTTP 204

      // Nothing saved to DB (No changes)
      return BadRequest("User not updated");
    }

    [HttpPost("add-photo")]
    // adding a route parameter to our HttpPost we already have (This will allow users to upload picture)
    // Should get a HTTP 201 created response

    //TODO: 1. Separate this larger method into smaller methods -> can have logic of setting main Photo as a service layer or domain logic. 2. More specific error handling, have console logs for specific issues (easier to debug). 
    public async Task<ActionResult<PhotoDto>> photoAdd(IFormFile file)
    {
      // get username from token
      var username = User.GetUsername();

      // Since we use our repo here, EF auto tracks the user
      var user = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(username);

      if (user == null) return NotFound("User not found in photoAdd method!"); // check in-case we don't have user (HTTP 404 Err)

      // Can absolute uri is stored in our DB (we use this to track the image in cloudinary)
      var imgUpload = await _servicePhoto.AsyncAddPhoto(file);


      // Check if we  have error with imgUpload
      if (imgUpload.Error != null) return BadRequest(imgUpload.Error.Message); //HHTP 400 Err

      var img = new Photo
      {
        Url = imgUpload.SecureUrl.AbsoluteUri,
        PublicId = imgUpload.PublicId
      };

      // Add photo to user collection -> EF still tracking user 
      user.Photos.Add(img);

      // save changes to DB 
      if (await _unitOfWork.TransactionComplete())
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
      var user = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(User.GetUsername());

      if (user == null) return NotFound("User not found, is null");

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); // can return null

      if (photo == null) return NotFound("id to match Photo not found");

      if (photo.IsMainPhoto) return BadRequest("Photo already set to Main photo");
      // In-case user uses 3rd party tool to update photo (app safety)

      var currMain = user.Photos.FirstOrDefault(x => x.IsMainPhoto);
      // Already have a main photo, we disable it / remove it via the bool flag
      if (currMain != null) currMain.IsMainPhoto = false;

      photo.IsMainPhoto = true;

      return await _unitOfWork.TransactionComplete() ? NoContent() : BadRequest("Main photo couldn't be updated!"); // HTTP 204
    }


    // Method to delete photos
    [HttpDelete("photo-delete/{photoId}")]

    public async Task<ActionResult> PhotoDelete(int photoId)
    {
      // User.GetUsername retrieves username from user token (only possible if authenticated)
      var username = User.GetUsername();

      // Get user from username
      var user = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(username);

      if (user == null) return NotFound("User not found to delete photo");

      // Get photo using its Id
      var photoToDelete = await _unitOfWork.PhotoRepository.PhotoByIdGetter(photoId);

      // Check if the photo exists
      if (photoToDelete == null) return NotFound("User photo not found");

      // Check if the photo is the main photo
      if (photoToDelete.IsMainPhoto) return BadRequest("This is main photo, cannot be deleted. Choose another main photo before deleting this one 😄");

      // Images without id are in our DB (from seeding --> used for testing, not in cloudinary so user can't access these)
      if (photoToDelete.PublicId != null)
      {
        var res = await _servicePhoto.AsyncDeletePhoto(photoToDelete.PublicId);

        if (res.Error != null) return BadRequest(res.Error.Message); // Error msg from cloudinary
      }

      user.Photos.Remove(photoToDelete); // EF auto updates DB

      if (await _unitOfWork.TransactionComplete()) return Ok(); // HTTP 200

      return BadRequest("Photo cannot be deleted -> Debug time :D");
    }




  }
}
