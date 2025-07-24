using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HRM_AI.Repositories.Common;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM_AI.Repositories.Repositories
{
    public class SystemConfigRepository : GenericRepository<SystemConfig>, ISystemConfigRepository
    {
        public SystemConfigRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<SystemConfig?> GetByEntityTypeAsync(string fieldName)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.FieldName == fieldName && x.IsActive == true);
        }

        public async Task<SystemConfig?> GetByKeyAsync(SystemConfigKey key)
        {
            if (!SystemConfiguration.ConfigKeys.TryGetValue(key, out string? fieldName))
                return null;

            return await _dbSet.FirstOrDefaultAsync(x => x.FieldName!.Equals(fieldName) && x.IsActive == true);
        }


        public async Task<string?> GetValueByKeyAsync(SystemConfigKey key)
        {
            if (!SystemConfiguration.ConfigKeys.TryGetValue(key, out string? fieldName))
                return null;

            var configValue = await _dbSet
                .Where(x => x.FieldName == fieldName && x.IsActive)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            return configValue?.Trim('"');

        }
    }
}
