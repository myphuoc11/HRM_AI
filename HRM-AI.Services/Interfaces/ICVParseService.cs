using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Interfaces
{
    public interface ICVParseService
    {
        Task<ResponseModel> ParseCVAsync(IFormFile file);
    }

}
