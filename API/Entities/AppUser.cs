using API.Extensions;

namespace API.Entities
{
  public class AppUser
  
  {
    public int Id { get; set; }

    public string UserName { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }
    
    public DateOnly DateOfBirth { get; set; }

    public string KnownAs { get; set; } 

    public DateTime UserCreated { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public string UserGender { get; set; }

    public string Introduction { get; set; }  

    public string LookingFor { get; set; }  

    public string UserInterests { get; set; }

    public string City { get; set; }  

    public string Country { get; set; }

    public List<Photo> Photos { get; set; } = new List<Photo>();


    // Many to many relationship between users

    // OTHER users who like our user
    public List<LikeUser> UserLikedBy { get; set; }
    // OUR user who likes OTHER users
    public List<LikeUser> UsersLiked { get; set; }

  }
}