using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace electric_mouse.Services.Interfaces
{
    public interface IAttachmentHandler
    {
        Task<string[]> SaveImagesOnServer(IList<IFormFile> images, string webRootPath, string folderName);
    }
}