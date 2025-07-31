using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.EmailModels;
using Nest;

namespace HRM_AI.Services.Interfaces
{
    public interface IEmailService
    {
        Task<ResponseModel> Update(EmailUpdateModel emailUpdateModel, Guid id);
        Task<ResponseModel> Get(EmailType type);

    }
}
