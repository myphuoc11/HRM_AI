using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.InterviewOutcomeAddModels;

namespace HRM_AI.Services.Interfaces
{
    public interface IInterviewOutcomeService
    {
        Task<ResponseModel> Add(InterviewOutcomeAddModel interviewOutcomeAddModel);
    }
}
