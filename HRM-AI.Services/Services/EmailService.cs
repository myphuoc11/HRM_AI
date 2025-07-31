using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.EmailModels;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;

        public EmailService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public async Task<ResponseModel> Get(EmailType type)
        {
            try
            {
              

                var email = await _unitOfWork.EmailRepository.Get(type);

                if (email == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Email template not found.",
                        Data = null
                    };
                }

                return new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Email template fetched successfully.",
                    Data = new
                    {
                        email.Id,
                        email.Type,
                        email.Subject,
                        email.Body
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching the email template.",
                    Data = ex.Message
                };
            }
        }


        public async Task<ResponseModel> Update(EmailUpdateModel emailUpdateModel, Guid id)
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

                if (string.IsNullOrWhiteSpace(emailUpdateModel.Subject))
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Subject cannot be empty."
                    };
                }

                if (string.IsNullOrWhiteSpace(emailUpdateModel.Body))
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Body cannot be empty."
                    };
                }

                var allowedPlaceholders = new[] {
                    "{{CandidateName}}",
                    "{{StartTime}}",
                    "{{EndTime}}",
                    "{{InterviewType}}",
                    "{{Round}}",
                    "{{Notes}}",
                    "{{CreatedByName}}",
                    "{{Interviewers}}"
                };

                var matches = Regex.Matches(emailUpdateModel.Body, @"{{\s*[^}]+\s*}}");
                foreach (Match match in matches)
                {
                    var placeholder = match.Value.Trim();
                    if (!allowedPlaceholders.Contains(placeholder))
                    {
                        return new ResponseModel
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Message = $"Invalid placeholder: {placeholder}"
                        };
                    }
                }

                var oldEmail = await _unitOfWork.EmailRepository.GetAsync(id);
                if (oldEmail == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Email template not found."
                    };
                }

                oldEmail.IsDeleted = true;
                oldEmail.ModificationDate = DateTime.UtcNow;
                oldEmail.ModifiedById = currentUserId;

                _unitOfWork.EmailRepository.Update(oldEmail);

                var newEmail = new Email
                {
                    Subject = emailUpdateModel.Subject,
                    Body = emailUpdateModel.Body,
                    Type = oldEmail.Type,
                    CreatedById = currentUserId
                };

                await _unitOfWork.EmailRepository.AddAsync(newEmail);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Email template updated (new version created).",
                    Data = new { newEmail.Id }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the email template.",
                    Data = ex.Message
                };
            }
        }


    }
}
