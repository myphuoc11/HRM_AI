using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Enums;

namespace HRM_AI.Repositories.Common
{
    public class SystemConfiguration
    {
        public static readonly Dictionary<SystemConfigKey, string> ConfigKeys = new()
        {
            { SystemConfigKey.VerificationCodeLength, "VerificationCodeLength" },
            { SystemConfigKey.VerificationCodeValidity, "VerificationCodeValidityInMinutes" },
            { SystemConfigKey.DefaultMaxPageSize, "DefaultMaxPageSize" },
            { SystemConfigKey.DefaultMinPageSize, "DefaultMinPageSize" },
            { SystemConfigKey.CVRatingScale, "CVRatingScale" },
        };
    }
}
