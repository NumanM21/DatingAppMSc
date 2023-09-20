namespace API.DTOs
{
  public class PhotoDto
  {
    public int Id { get; set; }

    public string photoUrl { get; set; } 
    
    public bool IsMainPhoto { get; set; }

    public bool IsPhotoApproved { get; set; }

    
  }
}