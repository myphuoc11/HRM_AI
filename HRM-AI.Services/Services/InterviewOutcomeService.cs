using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.InterviewOutcomeAddModels;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Services
{
    public class InterviewOutcomeService : IInterviewOutcomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;

        public InterviewOutcomeService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public async Task<ResponseModel> Add(InterviewOutcomeAddModel interviewOutcomeAddModel)
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
                var newOutcome = new InterviewOutcome
                {
                    InterviewScheduleId = interviewOutcomeAddModel.InterviewScheduleId,
                    Feedback = interviewOutcomeAddModel?.Feedback ?? "",
                };
                await _unitOfWork.InterviewOutcomeRepository.AddAsync(newOutcome);
                var result = await _unitOfWork.SaveChangeAsync();
                return result > 0 ? new ResponseModel
                {
                    Code = StatusCodes.Status201Created,
                    Message = "InterviewOutcome added successfully",
                    Data = newOutcome
                }
                    : new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Failed to add InterviewOutcome"
                    };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the InterviewOutcome.",
                    Data = ex.Message
                };
            }
        }
    }
}
