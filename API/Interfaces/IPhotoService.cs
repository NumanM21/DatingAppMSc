using CloudinaryDotNet.Actions;
namespace API.Interfaces
{
    // Interface which our photo api service will implement 
    public interface IPhotoService
    {
      Task<ImageUploadResult> AsyncAddPhoto(IFormFile file);

       Task<DeletionResult> AsyncDeletePhoto(string publicId); // will store this url string in our DB (our Photo DB has a publicId field for this--> How we store specific images in Cloudinary)
    }
}