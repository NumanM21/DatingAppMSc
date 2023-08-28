using API.ExternalHelpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{

  public class ServicePhoto : IPhotoService
  {

    private readonly Cloudinary _cloud;

    public ServicePhoto(IOptions<SettingsCloudinary> configuration)
    {
        var account = new Account // Creates our account, using config detail we put into appsetting.json (notupdated in our git repo-> Private.json--- Access these through our external helpers, settingCloudinary)
        (
            configuration.Value.CloudName,
            configuration.Value.APIKey,
            configuration.Value.APISecret
        );

        _cloud = new Cloudinary(account); // _cloud variable name for our account
    }
    public async Task<ImageUploadResult> AsyncAddPhoto(IFormFile file)
    {
      var resultUpload = new ImageUploadResult();

      if (file.Length > 0) // see if we have a file
      {
        // Using --> variable deleted once method completes -> memory efficient --> Helps with responsiveness of web server 
        using var stream = file.OpenReadStream();
        var paramaterUpload = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),  // want only square images (keep it simple for now, can TODO: change later == Crop to fill, and cropping 'centers' around the persons face since these are  images)
            Folder = "MSc-DatingApplication-UserPhoto"
        };

        resultUpload = await _cloud.UploadAsync(paramaterUpload); //ParamterUpload contains file and configuration parameters (transformation stuff)
      }
      return resultUpload;
    }

    public async Task<DeletionResult> AsyncDeletePhoto(string publicId)
    {
      var parameterDelete = new DeletionParams(publicId);

      return await _cloud.DestroyAsync(parameterDelete); //DestroyAsync returns DeletionResult
    }
  }
}