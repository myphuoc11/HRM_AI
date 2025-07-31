using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM_AI.Repositories.Repositories
{
    public class InterviewScheduleRepository : GenericRepository<Entities.InterviewSchedule>, Interfaces.IInterviewScheduleRepository
    {
        public InterviewScheduleRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<bool> CheckExistSchedule(Guid interviewerId, DateTime startTime, DateTime endTime)
        {
            return await _dbSet.Include(i => i.Interviewers).AnyAsync(i =>
                            i.Interviewers.Any(itv => itv.Id == interviewerId) &&
                            i.StartTime < endTime &&
                            (i.EndTime == null || startTime < i.EndTime));
        }
    }
}
