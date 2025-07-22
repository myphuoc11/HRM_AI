using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        AppDbContext Context { get; }

        public Task<int> SaveChangeAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        #region Repository
        IAccountRepository AccountRepository { get; }
        IAccountRoleRepository AccountRoleRepository { get; }
        IRoleRepository RoleRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        ICampaignPositionDetailRepository CampaignPositionDetailRepository { get; }
        ICampaignPositionRepository CampaignPositionRepository { get; }
        ICampaignRepository CampaignRepository { get; }
        ICVApplicantDetailsRepository CVApplicantDetailsRepository { get; }
        ICVApplicantRepository CVApplicantRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        IInterviewScheduleRepository InterviewScheduleRepository { get; }
        IInterviewerRepository InterviewerRepository { get; }
        IInterviewOutcomeRepository InterviewOutcomeRepository { get; }
        #endregion
    }
}
