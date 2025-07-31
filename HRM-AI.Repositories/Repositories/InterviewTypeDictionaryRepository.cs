using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Interfaces;

namespace HRM_AI.Repositories.Repositories
{
    public class InterviewTypeDictionaryRepository : GenericRepository<Entities.InterviewTypeDictionary>, Interfaces.IInterviewTypeDictionaryRepository
    {
        public InterviewTypeDictionaryRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }
    }
}
