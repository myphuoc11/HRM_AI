﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Common
{
    public static class Constant
    {
        #region Security

        // public const int AccessTokenValidityInMinutes = 5;
        public const int AccessTokenValidityInMinutes = 60 * 24 * 7;
        public const int RefreshTokenValidityInDays = 7;
        public const int VerificationCodeValidityInMinutes = 15;
        public const int VerificationCodeLength = 6;
        public const int ResetPasswordTokenValidityInMinutes = 15;

        #endregion

        #region Pagination

        // Default
        public const int DefaultMinPageSize = 10;
        public const int DefaultMaxPageSize = 50;

        // Conversation
        public const int ConversationMaxPageSize = 20;

        // Message
        public const int MessageMinPageSize = 20;
        public const int MessageMaxPageSize = 100;

        #endregion
    }
}
