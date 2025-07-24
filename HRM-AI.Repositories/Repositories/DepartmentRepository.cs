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
    public class DepartmentRepository : GenericRepository<Entities.Department>, Interfaces.IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<Department?> FindByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.Code == code.ToLower().Trim());
        }
    }
}
