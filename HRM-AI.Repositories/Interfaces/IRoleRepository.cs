using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> FindByNameAsync(string name);
        Task<List<Role>> GetAllByAccountIdAsync(Guid accountId);
    }
}
