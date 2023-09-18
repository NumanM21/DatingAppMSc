using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
  [Table("Photos")]
  public class Photo
  {
    public int Id { get; set; }

    public string Url { get; set; }

    public bool IsMainPhoto { get; set; }

    public string PublicId { get; set; }
    public bool IsPhotoApproved { get; set; }

    // This creates the specific migration we want between AppUser and Photos 
    public int AppUserId { get; set; }

    public AppUser AppUser { get; set; }
  }
}