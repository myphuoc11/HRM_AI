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
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<Role?> FindByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(role => role.Name == name);
        }

        public async Task<List<Role>> GetAllByAccountIdAsync(Guid accountId)
        {
            return await _dbSet.Where(role => role.AccountRoles.Any(accountRole => accountRole.AccountId == accountId))
                .ToListAsync();
        }
    }
}
