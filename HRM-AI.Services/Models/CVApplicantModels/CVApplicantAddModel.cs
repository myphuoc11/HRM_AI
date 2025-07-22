using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Models.CVApplicantModels
{
    public class CVApplicantAddModel
    {
        public required IFormFile FileUrl { get; set; } = null!;
        public required string FileAlt { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public required string ContactName { get; set; } = null!;
        public required string ContactValue { get; set; } = null!;
        public required Guid CampaignPositionId { get; set; }
        public List<CVApplicantAddModel> CVApplicantAddModels { get; set; } = new List<CVApplicantAddModel>();
    }
}
