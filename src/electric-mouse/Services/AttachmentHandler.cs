using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Services
{
    public class AttachmentHandler
    {
        
        /// <summary>
        /// Saves the given images based on the specified webRootPath and folderName.
        /// </summary>
        /// <param name="images">The images that should be saved.</param>
        /// <param name="webRootPath">The web root path.</param>
        /// <param name="folderName">The folder relative to the webRootPath where the images are to be saved.</param>
        /// <returns>Returns the relative paths in which where the files were saved.</returns>
        public async Task<string[]> SaveImagesOnServer(IList<IFormFile> images, string webRootPath, string folderName)
        {
            if (images == null) // bail if there are no images being uploaded
                return null;

            // the name of the folder to upload all the images
            string uploadFolderName = folderName;
            // generate a random file name for all the images that are being uploaded
            string[] imageFileNames = images.Select(image => image.FileName.GetRandomFileNameWithOriginalExtension()).ToArray();
            // get all the relative paths (uploads\<filename>)
            string[] relativeImagePaths = imageFileNames.Select(filename => Path.Combine(uploadFolderName, filename)).ToArray();
            // get the path to the uploads folder on the server
            string uploadFolderPath = Path.Combine(webRootPath, uploadFolderName);
            // get the full path (c:\...\wwwroot\uploads\<filename>)
            string[] fullImagePaths = imageFileNames.Select(filename => Path.Combine(uploadFolderPath, filename)).ToArray();
            int i = 0;

            // create uploads folder if it doesnt exist
            if (Directory.Exists(uploadFolderPath) == false)
                Directory.CreateDirectory(uploadFolderPath);

            foreach (IFormFile image in images)
            {
                if (image.Length <= 0 && image.Length > 5L.ConvertMegabytesToBytes()) // image size should not exceed 5 megabytes
                    continue; // skip the iteration; dont upload the image

                if (image.ContentType.Contains("image") == false)
                    continue; // if it isnt an image being uploaded; skip it!

                if (image.FileName.HasExtension(".png", ".jpg", ".jpeg") == false)
                    continue; // if it doesnt have an image extension; skip it!

                using (FileStream fileStream = new FileStream(fullImagePaths[i], FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
                i++;
            }

            return relativeImagePaths;
        }
    }
}
