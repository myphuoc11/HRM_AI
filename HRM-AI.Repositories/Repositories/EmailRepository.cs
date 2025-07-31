using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM_AI.Repositories.Repositories
{
    public class EmailRepository : GenericRepository<Entities.Email>, Interfaces.IEmailRepository
    {
        public EmailRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<Email> Get(EmailType emailType)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Type == emailType && e.IsDeleted == false)
                   ?? throw new InvalidOperationException("Email not found.");
        }
    }
}
