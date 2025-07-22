using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Repositories.Models.CampaignModels;
using HRM_AI.Repositories.Models.CampaignPositionModels;
using HRM_AI.Services.Common;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CampaignModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace HRM_AI.Services.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;

        public CampaignService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public async Task<ResponseModel> Add(CampaignAddModel campaignAddModel)
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

                var newCampaign = new Campaign
                {
                    Name = campaignAddModel.Name,
                    Description = campaignAddModel.Description,
                    StartTime = campaignAddModel.StarTime,
                    EndTime = campaignAddModel.EndTime
                };
                await _unitOfWork.CampaignRepository.AddAsync(newCampaign);
                var result = await _unitOfWork.SaveChangeAsync();
                return result > 0 ? new ResponseModel
                {
                    Code = StatusCodes.Status201Created,
                    Message = "Campaign added successfully",
                    Data = newCampaign
                }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to add campaign"
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
                var campaign = await _unitOfWork.CampaignRepository.GetAsync(id);
                if(campaign == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign not found"
                    };
                }
                _unitOfWork.CampaignRepository.SoftRemove(campaign);
                var result = await _unitOfWork.SaveChangeAsync();
                return result > 0 ? new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Campaign deleted successfully"
                }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to delete campaign"
                    };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while deleting the campaign.",
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseModel> GetAll(CampaignFilterModel campaignFilterModel)
        {
            try
            {
                var currentUserId = _claimService.GetCurrentUserId;
                if (!currentUserId.HasValue)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized"
                    };
                }
                Expression<Func<Campaign, bool>> filterExpression = request =>
                (request.IsDeleted == campaignFilterModel.IsDeleted) &&
                    (!campaignFilterModel.StartTime.HasValue || request.StartTime == campaignFilterModel.StartTime) &&
                    (!campaignFilterModel.StartTime.HasValue || request.StartTime == campaignFilterModel.StartTime) &&
                    (string.IsNullOrEmpty(campaignFilterModel.Search) || request.Name!.ToLower().Contains(campaignFilterModel.Search.ToLower()));

                Func<IQueryable<Campaign>, IQueryable<Campaign>> includeWithOrder = query =>
                {
                    var campaigns = campaignFilterModel.OrderByDescending
                        ? query.OrderByDescending(_ => _.CreationDate)
                        : query.OrderBy(_ => _.CreationDate);
                    return campaigns;
                };
                var requestsResult = await _unitOfWork.CampaignRepository.GetAllAsync(
                filter: filterExpression,
                include: includeWithOrder,
                pageIndex: campaignFilterModel.PageIndex,
                pageSize: campaignFilterModel.PageSize
                );
                var campaignModels = requestsResult.Data.Select(_ => new CampaignModel
                {
                    Id = _.Id,
                    Name = _.Name,
                    Description = _.Description,
                    StartTime = _.StartTime,
                    EndTime = _.EndTime,
                    CreationDate = _.CreationDate
                }).ToList();
                var result = new Pagination<CampaignModel>(
                    campaignModels,
                    campaignFilterModel.PageIndex,
                    campaignFilterModel.PageSize,
                    requestsResult.TotalCount
                );

                return new ResponseModel
                {
                    Message = "Get all requests successfully",
                    Data = result
                };

            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving campaigns.",
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseModel> GetById(Guid id)
        {
            try
            {
                var campaign = await _unitOfWork.CampaignRepository.GetAsync(id);
                if (campaign == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign not found"
                    };
                }
                var campaignModel = new CampaignModel
                {
                    Id = campaign.Id,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    StartTime = campaign.StartTime,
                    EndTime = campaign.EndTime,
                    CreationDate = campaign.CreationDate,
                    CampaignPositions = campaign.CampaignPositions.Select(cp => new CampaignPositionModel
                    {
                        Id = cp.Id,
                        DepartmentId = cp.DepartmentId,
                        DepartmentName = cp.Department.DepartmentName,
                        TotalSlot = cp.TotalSlot,
                        Description = cp.Description,
                    }).ToList()

                };
                return new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Campaign retrieved successfully",
                    Data = campaignModel
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving the campaign.",
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseModel> Update(Guid id, CampaignUpdateModel campaignUpdateModel)
        {
            try 
            {
                var campaign = await _unitOfWork.CampaignRepository.GetAsync(id);
                if (campaign == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Campaign not found"
                    };
                }
                campaign.Name = campaignUpdateModel.Name;
                campaign.Description = campaignUpdateModel.Description;
                campaign.StartTime = campaignUpdateModel.StartTime;
                campaign.EndTime = campaignUpdateModel.EndTime;
                _unitOfWork.CampaignRepository.Update(campaign);
                var result = await _unitOfWork.SaveChangeAsync();
                return result > 0 ? new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Campaign updated successfully",
                    Data = campaign
                }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to update campaign"
                    };

            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving campaigns.",
                    Data = ex.Message
                };
            }
        }
    }
}
