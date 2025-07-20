using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Repositories.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRM_AI.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction _transaction;
        public UnitOfWork(AppDbContext context,
            IAccountRepository accountRepository,
             IAccountRoleRepository accountRoleRepository,
             IRoleRepository roleRepository,
          IRefreshTokenRepository refreshTokenRepository

            )
        {
            Context = context;
            AccountRepository = accountRepository;
            AccountRoleRepository = accountRoleRepository;
            RoleRepository = roleRepository;
            RefreshTokenRepository = refreshTokenRepository;
        }

        public AppDbContext Context { get; }
        public IAccountRepository AccountRepository { get; }
        public IAccountRoleRepository AccountRoleRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }


        public async Task<int> SaveChangeAsync()
        {
            return await Context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await Context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
