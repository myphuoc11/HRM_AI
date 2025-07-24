using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.SystemConfigModels;

namespace HRM_AI.Services.Interfaces
{
    public interface ISystemConfigService
    {
        Task<ResponseModel> Add(SystemConfigAddModel model);
        Task<ResponseModel> Update(Guid id, SystemConfigUpdateModel model);
        Task<ResponseModel> GetAll(SystemConfigFilterModel model);
        Task<ResponseModel> Get(SystemConfigKey key);
        Task<List<object[]>> GetAllAsKeyValueAsync();
    }
}
