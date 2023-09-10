using API.DTOs;
using API.Entities;
using API.ExternalHelpers;

namespace API.Interfaces
{
    public interface ILikeRepository
    {

        // entities which make up the PK of the LikeUser table
        Task<LikeUser> GetLikeUser(int SourceUserID, int UserLikedBySourceID);

        Task<AppUser> GetUserWithLikes(int userId);

        // predicate is the filter we want to apply to the query, e.g. if we want to get all the users who have liked our user or all the users our user has liked
      
        Task<PaginationList<LikeUserDto>> GetUserLikes(ParameterLikes paramLikes);
        
    }
}