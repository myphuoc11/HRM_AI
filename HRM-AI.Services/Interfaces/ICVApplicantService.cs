﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CVApplicantModels;

namespace HRM_AI.Services.Interfaces
{
    public interface ICVApplicantService
    {
        Task<ResponseModel> Add(CVApplicantAddModel cVApplicantAddModel);
    } 
}
