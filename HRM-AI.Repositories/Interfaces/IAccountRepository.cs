using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {

        Task<Account?> FindByEmailAsync(string email, Func<IQueryable<Account>, IQueryable<Account>>? include = null);
        Task<Account?> FindByUsernameAsync(string username, Func<IQueryable<Account>, IQueryable<Account>>? include = null);
        Task<List<Guid>> GetValidAccountIdsAsync(List<Guid> accountIds);
    }
}
