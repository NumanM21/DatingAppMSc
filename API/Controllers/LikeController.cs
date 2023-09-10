
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class LikeController : BaseApiController
  {
    private readonly ILikeRepository _likeRepo;
    private readonly IUserRepository _userRepo;
    public LikeController(IUserRepository userRepo, ILikeRepository likeRepo)
    {
      _userRepo = userRepo;
      _likeRepo = likeRepo;

    }

    // Creating new resource for when a user likes another user (username of person they are GOING to like) -> Updating join table! 
    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
      // user liking another user
      var sourceUserId = User.GetUserId();

      // user being liked
      var userBeingLiked = await _userRepo.AsyncGetUserByUsername(username);
      var sourceUser = await _likeRepo.GetUserWithLikes(sourceUserId);

      // if user being liked does not exist
      if (userBeingLiked == null) return NotFound("User being liked does not exist");
      
      // if user being liked is the same as the user liking
      if (sourceUser.UserName == username) return BadRequest("Cannot like yourself"); // incase user uses 3rd party tool to send request (security)

      // check if user has already liked the user they are trying to like
      var userLiked = await _likeRepo.GetUserLikes(sourceUserId, userBeingLiked.Id);
      if (userLiked != null) return BadRequest("User already liked");


      // create new like

      userLiked = new LikeUser
      {
        SourceUserID = sourceUserId, 
        UserLikedBySourceID = userBeingLiked.Id
      };

      // add like to db
      sourceUser.UsersLiked.Add((LikeUser)userLiked);

      if (await _userRepo.AsyncSaveAll()) return Ok();

      return BadRequest("Failed to save like");

    }


  }
}