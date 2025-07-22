using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CampaignPositionModels;

namespace HRM_AI.Services.Interfaces
{
    public interface ICampaignPositionService
    {
        Task<ResponseModel> Add(CampaignPositionAddModel campaignPositionAddModel);
        Task<ResponseModel> Update(CampaignPositionUpdateModel campaignPositionUpdateModel, Guid id);
        Task<ResponseModel> Delete(Guid id);

    }
}
