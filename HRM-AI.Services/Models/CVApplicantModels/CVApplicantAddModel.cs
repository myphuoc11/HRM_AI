using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using HRM_AI.Services.Models.CVApplicantDetails;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Models.CVApplicantModels
{
    public class CVApplicantAddModel
    {
        public required IFormFile FileUrl { get; set; } = null!;
        public required string FileAlt { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Point { get; set; } = null!;
        public required Guid CampaignPositionId { get; set; }
        public List<CVApplicantDetailsAddModel> CVApplicantDetailsAddModels { get; set; } = new List<CVApplicantDetailsAddModel>();
    }
}
