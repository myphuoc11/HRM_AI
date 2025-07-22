using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CampaignModels;

namespace HRM_AI.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<ResponseModel> Add(CampaignAddModel campaignAddModel);
        Task<ResponseModel> GetAll(CampaignFilterModel campaignFilterModel);
        Task<ResponseModel> Update(Guid id, CampaignUpdateModel campaignUpdateModel);
        Task<ResponseModel> Delete(Guid id);
        Task<ResponseModel> GetById(Guid id);
    }
}
