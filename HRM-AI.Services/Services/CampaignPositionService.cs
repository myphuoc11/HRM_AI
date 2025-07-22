using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CampaignPositionModels;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Services
{
    public class CampaignPositionService : ICampaignPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenAiService _openAiService;
        private readonly IClaimService _claimService;

        public CampaignPositionService(IUnitOfWork unitOfWork, IOpenAiService openAiService, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _openAiService = openAiService;
            _claimService = claimService;
        }

        public async Task<ResponseModel> Add(CampaignPositionAddModel campaignPositionAddModel)
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
                var findedCampaign = await _unitOfWork.CampaignRepository.GetAsync(campaignPositionAddModel.CampaignId);
                if (findedCampaign == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign not found."
                    };
                }
                var findedPosition = await _unitOfWork.DepartmentRepository.GetAsync(campaignPositionAddModel.DepartmentId);
                if (findedPosition == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Department not found."
                    };
                }
                var newCampaignPosition = new CampaignPosition
                {
                    CampaignId = campaignPositionAddModel.CampaignId,
                    DepartmentId = campaignPositionAddModel.DepartmentId,
                    Description = campaignPositionAddModel.Description,
                    TotalSlot = campaignPositionAddModel.TotalSlot ?? 0,
                    CreatedById = currentUserId.Value,
                    CampaignPositionDetails = campaignPositionAddModel.campaignPositionDetailAddModels
                        .Select(detail => new CampaignPositionDetail
                        {
                            Type = detail.Type,
                            Key = detail.Key,
                            Value = detail.Value,
                            GroupIndex = detail.GroupIndex
                        }).ToList()
                };
                var jdParts = new List<string>
                {
                    campaignPositionAddModel.Description
                };

                jdParts.AddRange(campaignPositionAddModel.campaignPositionDetailAddModels
                    .Select(d => $"{d.Key}: {d.Value}"));

                string jdText = string.Join("\n", jdParts);

                var embeddingVector = await _openAiService.GetEmbeddingAsync(new List<string> { jdText });
                newCampaignPosition.Embedding = embeddingVector;

                await _unitOfWork.CampaignPositionRepository.AddAsync(newCampaignPosition);
                var result = await _unitOfWork.SaveChangeAsync();
                return result > 0
                    ? new ResponseModel
                    {
                        Code = StatusCodes.Status200OK,
                        Message = "Campaign position added successfully.",
                        Data = newCampaignPosition
                    }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to add campaign position."
                    };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the campaign.",
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseModel> Delete(Guid id)
        {
            try
            {
                var existingCampaignPosition = await _unitOfWork.CampaignPositionRepository.GetAsync(id);
                if (existingCampaignPosition == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign position not found."
                    };
                }
                _unitOfWork.CampaignPositionRepository.SoftRemove(existingCampaignPosition);
                var result = await _unitOfWork.SaveChangeAsync();
                return result > 0
                    ? new ResponseModel
                    {
                        Code = StatusCodes.Status200OK,
                        Message = "Campaign position deleted successfully."
                    }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to delete campaign position."
                    };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while deleting the campaign position.",
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseModel> Update(CampaignPositionUpdateModel campaignPositionUpdateModel, Guid id)
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

                var existingCampaignPosition = await _unitOfWork.CampaignPositionRepository.GetAsync(id);
                if (existingCampaignPosition == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign position not found."
                    };
                }

                var findedCampaign = await _unitOfWork.CampaignRepository.GetAsync(campaignPositionUpdateModel.CampaignId);
                if (findedCampaign == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign not found."
                    };
                }

                var findedDepartment = await _unitOfWork.DepartmentRepository.GetAsync(campaignPositionUpdateModel.DepartmentId);
                if (findedDepartment == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Department not found."
                    };
                }

                existingCampaignPosition.CampaignId = campaignPositionUpdateModel.CampaignId;
                existingCampaignPosition.DepartmentId = campaignPositionUpdateModel.DepartmentId;
                existingCampaignPosition.Description = campaignPositionUpdateModel.Description;
                existingCampaignPosition.TotalSlot = campaignPositionUpdateModel.TotalSlot ?? existingCampaignPosition.TotalSlot;
                existingCampaignPosition.ModifiedById = currentUserId.Value;

                existingCampaignPosition.CampaignPositionDetails.Clear();
                existingCampaignPosition.CampaignPositionDetails = campaignPositionUpdateModel.campaignPositionDetailAddModels
                    .Select(detail => new CampaignPositionDetail
                    {
                        Type = detail.Type,
                        Key = detail.Key,
                        Value = detail.Value,
                        GroupIndex = detail.GroupIndex
                    }).ToList();

                _unitOfWork.CampaignPositionRepository.Update(existingCampaignPosition);
                var result = await _unitOfWork.SaveChangeAsync();

                return result > 0
                    ? new ResponseModel
                    {
                        Code = StatusCodes.Status200OK,
                        Message = "Campaign position updated successfully.",
                        Data = existingCampaignPosition
                    }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to update campaign position."
                    };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the campaign position.",
                    Data = ex.Message
                };
            }
        }
    }
    }
