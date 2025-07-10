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
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<RefreshToken?> FindByDeviceIdAsync(Guid deviceId)
        {
            return await _dbSet.FirstOrDefaultAsync(refreshToken => refreshToken.DeviceId == deviceId);
        }
    }
}
