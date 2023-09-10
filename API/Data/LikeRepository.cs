using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class LikeRepository : ILikeRepository
  {
    private readonly DataContext _context;

    public LikeRepository(DataContext context)
    {
      _context = context;

    }

    // Find userLike entity with the PK of the two users
    public async Task<LikeUser> GetLikeUser(int SourceUserID, int UserLikedBySourceID)
    {
      return await _context.Like.FindAsync(SourceUserID, UserLikedBySourceID);
    }


    public async Task<IEnumerable<LikeUserDto>> GetUserLikes(string predicate, int userID)
    {
      // need to build our query (not hitting our DB!)
      var users = _context.Users.OrderBy(x => x.UserName).AsQueryable(); // queryable so we can add more to it (get all users)

      var like = _context.Like.AsQueryable(); //  not being executed yet

      // if we want to return users our user has liked
      if (predicate == "liked")
      {
        like = like.Where(like => like.SourceUserID == userID);
        // refining our users query to only return users our user has liked
        users = like.Select(like => like.UserLikedBySource);
      }
      // return users who have liked our user
      if (predicate == "likedby")
      {
        like = like.Where(like => like.UserLikedBySourceID == userID);
        // refining our users query to only return users who have liked our user
        users = like.Select(like => like.SourceUser);
      }

      return await users.Select(user => new LikeUserDto
      {
        UserName = user.UserName,
        Id = user.Id,
        KnownAs = user.KnownAs,
        Age = user.DateOfBirth.CalculateAge(),
        photoUrl = user.Photos.FirstOrDefault(x => x.IsMainPhoto).Url,
        City = user.City
      }).ToListAsync();

    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
      return await _context.Users
      .Include(x => x.UsersLiked)
      .FirstOrDefaultAsync(x => x.Id == userId);
    }
  }
}