using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.AccountModels
{
    public class AccountRefreshTokenModel
    {
        public Guid? DeviceId { get; set; }
        public string? AccessToken { get; set; }
        public Guid? RefreshToken { get; set; }
    }
}
