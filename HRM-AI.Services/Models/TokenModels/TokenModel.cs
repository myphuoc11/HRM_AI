using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.TokenModels
{
    public class TokenModel
    {
        public Guid DeviceId { get; set; }
        public string AccessToken { get; set; } = null!;
        public Guid RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}
