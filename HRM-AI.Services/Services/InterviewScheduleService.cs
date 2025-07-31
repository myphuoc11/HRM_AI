using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Helpers;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.InterviewScheduleModels;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Services
{
    public class InterviewScheduleService : IInterviewScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        private readonly IEmailHelper _iIEmailHelper;

        public InterviewScheduleService(IUnitOfWork unitOfWork, IClaimService claimService, IEmailHelper iIEmailHelper)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
            _iIEmailHelper = iIEmailHelper;
        }

        public async Task<ResponseModel> Add(InterviewScheduleAddModel model)
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
                var applicant = await _unitOfWork.CVApplicantRepository.GetAsync(model.CVApplicantId);
                if (applicant == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "CV applicant not found.",
                        Data = null
                    };
                }

                var interviewType = await _unitOfWork.InterviewTypeDictionaryRepository.GetAsync(model.InterviewTypeId);
                if (interviewType == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Interview type not found.",
                        Data = null
                    };
                }

                if (model.EndTime.HasValue && model.StartTime >= model.EndTime.Value)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Start time must be earlier than end time.",
                        Data = null
                    };
                }

                var interviewers = await _unitOfWork.AccountRepository
                    .GetAllAsync(filter: a => model.InterviewerIds.Contains(a.Id));
                if (interviewers.Data.Count != model.InterviewerIds.Count)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "One or more interviewers not found.",
                        Data = null
                    };
                }

                foreach (var interviewerId in model.InterviewerIds)
                {
                    var conflict = await _unitOfWork.InterviewScheduleRepository.CheckExistSchedule(interviewerId, model.StartTime, (DateTime)model.EndTime);

                    if (conflict)
                    {
                        return new ResponseModel
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Message = $"Interviewer with ID {interviewerId} has a conflicting schedule.",
                            Data = null
                        };
                    }
                }

              
                var creator = await _unitOfWork.AccountRepository.GetAsync(currentUserId.Value);
                if (creator == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Creator account not found.",
                        Data = null
                    };
                }

                var schedule = new InterviewSchedule
                {
                    CVApplicantId = model.CVApplicantId,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Round = model.Round,
                    InterviewTypeId = model.InterviewTypeId,
                    Notes = model.Notes,
                    CreatedBy = creator,
                    Interviewers = model.InterviewerIds.Select(_ => new Interviewer
                    {
                        InterviewerAccountId = _,
                    }).ToList(),
                };

                await _unitOfWork.InterviewScheduleRepository.AddAsync(schedule);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    var template = await _unitOfWork.EmailRepository.Get(EmailType.InterviewReminder);
                    if (template != null)
                    {
                        string interviewersStr = string.Join(", ", interviewers.Data.Select(x => x.FirstName + " " +x.LastName));

                        var replacements = new Dictionary<string, string>
                        {
                            { "CandidateName", applicant.FullName },
                            { "InterviewType", interviewType.Name },
                            { "StartTime", model.StartTime.ToString("HH:mm dd/MM/yyyy") },
                            { "EndTime", model.EndTime?.ToString("HH:mm dd/MM/yyyy") ?? " " },
                            { "Round", (model.Round ?? 1).ToString() },
                            { "Notes", model.Notes ?? " " },
                            { "CreatedByName", creator.FirstName + " " + creator.LastName },
                            { "Interviewers", interviewersStr }
                        };


                        string renderedSubject = PlaceholderReplacer.Replace(template.Subject, replacements);
                        string renderedBody = PlaceholderReplacer.Replace(template.Body, replacements);

                        await _iIEmailHelper.SendEmailAsync(applicant.Email, renderedSubject, renderedBody, true);

                        foreach (var interviewer in interviewers.Data)
                        {
                            await _iIEmailHelper.SendEmailAsync(interviewer.Email, renderedSubject, renderedBody, true);
                        }
                    }

                    return new ResponseModel
                    {
                        Code = StatusCodes.Status200OK,
                        Message = "Interview schedule created successfully.",
                    };
                }
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "Fail to adding the campaign."
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


    }
}
