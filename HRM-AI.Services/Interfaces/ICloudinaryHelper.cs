using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Interfaces
{
    public interface ICloudinaryHelper
    {
        Task<string> UploadImageAsync(IFormFile file, string? name = null, string? publicId = null, bool? overwrite = true, string? folderName = null);
        Task<string> RemoveImagesAsync(List<string> publicIds);
    }
}
