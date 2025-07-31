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
          IRefreshTokenRepository refreshTokenRepository,
            ICampaignPositionDetailRepository campaignPositionDetailRepository,
            ICampaignPositionRepository campaignPositionRepository,
            ICampaignRepository campaignRepository,
            ICVApplicantDetailsRepository cVApplicantDetailsRepository,
            ICVApplicantRepository cVApplicantRepository,
            IDepartmentRepository departmentRepository,
            IInterviewScheduleRepository interviewScheduleRepository,
            IInterviewerRepository interviewerRepository,
            IInterviewOutcomeRepository interviewOutcomeRepository,
            ISystemConfigRepository systemConfigRepository,
            IInterviewTypeDictionaryRepository interviewTypeDictionaryRepository,
            IEmailRepository emailRepository
            )
        {
            Context = context;
            AccountRepository = accountRepository;
            AccountRoleRepository = accountRoleRepository;
            RoleRepository = roleRepository;
            RefreshTokenRepository = refreshTokenRepository;
            CampaignPositionDetailRepository = campaignPositionDetailRepository;
            CampaignPositionRepository = campaignPositionRepository;
            CampaignRepository = campaignRepository;
            CVApplicantDetailsRepository = cVApplicantDetailsRepository;
            CVApplicantRepository = cVApplicantRepository;
            DepartmentRepository = departmentRepository;
            InterviewScheduleRepository = interviewScheduleRepository;
            InterviewerRepository = interviewerRepository;
            InterviewOutcomeRepository = interviewOutcomeRepository;
            SystemConfigRepository = systemConfigRepository;
            InterviewTypeDictionaryRepository = interviewTypeDictionaryRepository;
            EmailRepository = emailRepository;
        }

        public AppDbContext Context { get; }
        public IAccountRepository AccountRepository { get; }
        public IAccountRoleRepository AccountRoleRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public ICampaignPositionDetailRepository CampaignPositionDetailRepository { get; }

        public ICampaignPositionRepository CampaignPositionRepository { get; }

        public ICampaignRepository CampaignRepository { get; }

        public ICVApplicantDetailsRepository CVApplicantDetailsRepository { get; }

        public ICVApplicantRepository CVApplicantRepository { get; }

        public IDepartmentRepository DepartmentRepository { get; }

        public IInterviewScheduleRepository InterviewScheduleRepository { get; }

        public IInterviewerRepository InterviewerRepository { get; }

        public IInterviewOutcomeRepository InterviewOutcomeRepository { get; }

        public ISystemConfigRepository SystemConfigRepository { get; }

        public IInterviewTypeDictionaryRepository InterviewTypeDictionaryRepository { get; }

        public IEmailRepository EmailRepository { get; }

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
