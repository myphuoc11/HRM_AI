using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Common
{
    public class ConfigKeyDisplayNames
    {
        public static readonly Dictionary<SystemConfigKey, string> DisplayNames = new()
        {
            // Thời gian hiệu lực
            { SystemConfigKey.VerificationCodeLength, "Độ dài mã xác thực" },
            { SystemConfigKey.VerificationCodeValidity, "Thời gian hiệu lực mã xác thực (phút)" },

            // Phân trang
            { SystemConfigKey.DefaultMaxPageSize, "Số mục tối đa mặc định/trang" },
            { SystemConfigKey.DefaultMinPageSize, "Số mục tối thiểu mặc định/trang" },

            { SystemConfigKey.CVRatingScale, "Thang điểm đánh giá CV của ứng viên" }
        };

    }
}
