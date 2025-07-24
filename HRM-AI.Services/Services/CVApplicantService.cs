using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CVApplicantModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRM_AI.Services.Services
{
    public class CVApplicantService : ICVApplicantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        private readonly IGoogleDriveService _googleDriveService;

        public CVApplicantService(IUnitOfWork unitOfWork, IClaimService claimService, IGoogleDriveService googleDriveService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
            _googleDriveService = googleDriveService;
        }

        public async Task<ResponseModel> Add(CVApplicantAddModel cVApplicantAddModel)
        {
            try
            {
                var currentUserId = _claimService.GetCurrentUserId;
                if (!currentUserId.HasValue)
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized"
                    };

                var file = cVApplicantAddModel.FileUrl;
                using var stream = file.OpenReadStream();
                var position = await _unitOfWork.CampaignPositionRepository.GetAsync(cVApplicantAddModel.CampaignPositionId, include: _ => _.Include(_ => _.Campaign));
                var campaign = await _unitOfWork.CampaignRepository.GetAsync(position.CampaignId);

                var campaignFolderName = campaign?.Name.Trim() ?? $"{campaign.Name.Trim()}";
                var positionFolderName = position?.Description.Trim() ?? $"{position?.Description.Trim()}";
                var fileDriveUrl = await _googleDriveService.UploadFileAsync(
                                    stream,
                                    file.FileName,
                                    file.ContentType,
                                    campaignFolderName,
                                    positionFolderName
                                );
                var cVRatingPoint = _unitOfWork.SystemConfigRepository.GetValueByKeyAsync(SystemConfigKey.CVRatingScale).Result;

                var newCVApplicant = new CVApplicant
                {
                    CampaignPositionId = cVApplicantAddModel.CampaignPositionId,
                    FullName = cVApplicantAddModel.FullName,
                    FileUrl = fileDriveUrl,
                    FileAlt = cVApplicantAddModel.FileAlt,
                    Point = cVApplicantAddModel.Point + $"/{cVRatingPoint}",
                    CVApplicantDetails = cVApplicantAddModel.CVApplicantDetailsAddModels
                        .Select(x => new CVApplicantDetails
                        {
                            Type = x.Type,
                            Key = x.Key,
                            Value = x.Value,
                            GroupIndex = x.GroupIndex,
                        }).ToList()
                };

               

                await _unitOfWork.CVApplicantRepository.AddAsync(newCVApplicant);
                var result = await _unitOfWork.SaveChangeAsync();

                return result > 0 ? new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Upload CV successfully",
                    Data = newCVApplicant
                } : new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "Upload CV failed",
                    Data = newCVApplicant
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "Tạo CV thất bại: " + ex.Message,
                    Data = null
                };
            }
        }

    }
}
