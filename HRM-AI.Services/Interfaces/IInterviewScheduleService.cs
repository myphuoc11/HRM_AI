using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.InterviewScheduleModels;

namespace HRM_AI.Services.Interfaces
{
    public interface IInterviewScheduleService
    {
        Task<ResponseModel> Add(InterviewScheduleAddModel interviewScheduleAddModel);
    }
}
