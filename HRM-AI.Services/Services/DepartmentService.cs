using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Helpers;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.DepartmentModels;
using Microsoft.AspNetCore.Http;

namespace HRM_AI.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        public DepartmentService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public async Task<ResponseModel> Add(DepartmentAddModel departmentAddModel)
        {
            try
            {
                string slug = GenerateSlug.CreateSlug(departmentAddModel.DepartmentName);
                var existingDepartment = await _unitOfWork.DepartmentRepository.FindByCodeAsync(slug);
                if (existingDepartment != null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Department code already exists.",
                        Data = null
                    };
                }

                var newDepartment = new Department
                {
                    DepartmentName = departmentAddModel.DepartmentName,
                    Code = slug,
                    Description = departmentAddModel.Description
                };

                await _unitOfWork.DepartmentRepository.AddAsync(newDepartment);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Department added successfully.",
                    Data = newDepartment
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the department.",
                    Data = ex.Message
                };
            }
        }

       
    }
}
