using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CVApplicantModels;

namespace HRM_AI.Services.Services
{
    public class CVApplicantService : ICVApplicantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;

        public CVApplicantService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public Task<ResponseModel> Add(CVApplicantAddModel cVApplicantAddModel)
        {
            throw new NotImplementedException();
        }
    }
}
