﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Models.EmailModels
{
    public class EmailUpdateModel
    {
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
