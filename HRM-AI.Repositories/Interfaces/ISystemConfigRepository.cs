using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Interfaces
{
    public interface ISystemConfigRepository : IGenericRepository<SystemConfig>
    {
        Task<SystemConfig?> GetByEntityTypeAsync(string fieldName);
        Task<SystemConfig?> GetByKeyAsync(SystemConfigKey key);
        Task<string?> GetValueByKeyAsync(SystemConfigKey key);
    }
}
