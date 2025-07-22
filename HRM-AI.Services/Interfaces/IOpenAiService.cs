using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Interfaces
{
    public interface IOpenAiService
    {
        Task<float[]> GetEmbeddingAsync(List<string> texts);
        Task<ResponseModel> ParseCVAsync(IFormFile file, Guid campaignPositionId);
    }
}
