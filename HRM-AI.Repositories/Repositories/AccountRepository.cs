﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM_AI.Repositories.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context, IClaimService claimService) : base(context, claimService)
        {
        }

        public async Task<Account?> FindByEmailAsync(string email,
            Func<IQueryable<Account>, IQueryable<Account>>? include = null)
        {
            IQueryable<Account> query = _dbSet;
            if (include != null) query = include(query);

            return await query.FirstOrDefaultAsync(account => account.Email == email);
        }

        public async Task<Account?> FindByUsernameAsync(string username,
            Func<IQueryable<Account>, IQueryable<Account>>? include = null)
        {
            IQueryable<Account> query = _dbSet;
            if (include != null) query = include(query);

            return await query.FirstOrDefaultAsync(account => account.Username == username);
        }

        public async Task<List<Guid>> GetValidAccountIdsAsync(List<Guid> accountIds)
        {
            return await _dbSet.Where(account => accountIds.Contains(account.Id) && !account.IsDeleted)
                .Select(account => account.Id).Distinct().ToListAsync();
        }
    }
}
