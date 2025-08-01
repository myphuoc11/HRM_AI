﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Repositories.Interfaces
{
    public interface IInterviewScheduleRepository : IGenericRepository<Entities.InterviewSchedule>
    {
        Task<bool> CheckExistSchedule(Guid interviewerId, DateTime startTime, DateTime endTime);
    }
}
