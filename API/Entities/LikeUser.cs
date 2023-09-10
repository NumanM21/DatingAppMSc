
namespace API.Entities
{
  //Act as a join table for many to many relationship between users (joining of two separate entities, but same AppUser entity type)
  public class LikeUser
  {
    public AppUser SourceUser { get; set; }
    public int SourceUserID { get; set; }
    public AppUser UserLikedBySource { get; set; } // another user OUR user likes
    public int UserLikedBySourceID { get; set; }

  }
}

