﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Common;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models.SystemConfigModels;
using HRM_AI.Services.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using HRM_AI.Repositories.Models.SystemConfigModels;
using HRM_AI.Repositories.Common;

namespace HRM_AI.Services.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemConfigService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> Add(SystemConfigAddModel model)
        {
            try
            {
                var existingConfig = await _unitOfWork.SystemConfigRepository.GetByEntityTypeAsync(model.FieldName);
                if (existingConfig != null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Configuration key already exists",
                        Data = false
                    };
                }

                var config = new SystemConfig
                {
                    FieldName = model.FieldName,
                    EntityType = model.EntityType,
                    Value = JsonSerializer.Serialize(model.Value) 
                };


                await _unitOfWork.SystemConfigRepository.AddAsync(config);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Code = StatusCodes.Status201Created,
                    Message = "Configuration added successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = $"An error occurred while adding configuration: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel> Update(Guid id, SystemConfigUpdateModel model)
        {
            try
            {
                var config = await _unitOfWork.SystemConfigRepository.GetAsync(id);
                if (config == null)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Message = "Configuration not found",
                        Data = false
                    };
                }

                var now = DateOnly.FromDateTime(DateTime.Now);
                var serializedValue = JsonSerializer.Serialize(model.Value); // ✅ chuyển sang string

                if ((model.EffectiveFrom == null || model.EffectiveFrom == now) && config.IsActive == true)
                {
                    config.Value = serializedValue;
                    _unitOfWork.SystemConfigRepository.Update(config);
                }
                else if (config.EffectiveFrom > now && config.IsActive == false)
                {
                    config.Value = serializedValue;

                    if (model.EffectiveFrom != null)
                        config.EffectiveFrom = model.EffectiveFrom;

                    _unitOfWork.SystemConfigRepository.Update(config);
                }
                else
                {
                    var newConfig = new SystemConfig
                    {
                        Id = Guid.NewGuid(),
                        FieldName = config.FieldName,
                        EntityType = config.EntityType,
                        Value = serializedValue,
                        EffectiveFrom = model.EffectiveFrom,
                        IsActive = false
                    };

                    await _unitOfWork.SystemConfigRepository.AddAsync(newConfig);
                }


                int result = await _unitOfWork.SaveChangeAsync();

                if (result > 0)
                {
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status200OK,
                        Message = "Configuration updated successfully",
                        Data = true
                    };
                }

                return new ResponseModel
                {
                    Code = StatusCodes.Status422UnprocessableEntity,
                    Message = "Nothing change",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = $"An error occurred while updating configuration: {ex.Message}",
                    Data = false
                };
            }
        }
        public async Task<ResponseModel> GetAll(SystemConfigFilterModel model)
        {
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var configList = await _unitOfWork.SystemConfigRepository.GetAllAsync(
        x =>
            !x.IsDeleted &&
            (!model.IsActive.HasValue || x.IsActive == model.IsActive) &&
        (!model.Type.HasValue || x.EntityType == model.Type) &&
        (!model.Past.HasValue || (model.Past.Value ? !x.IsActive && (x.EffectiveFrom == null || x.EffectiveFrom < now) : DateOnly.FromDateTime(x.CreationDate) < now)) &&
        (!model.Future.HasValue || (model.Future.Value && x.EffectiveFrom > now)),
        q =>
        {
            switch (model.OrderOption)
            {
                case SortOptions.EntityType:
                    return model.OrderByDescending
                        ? q.OrderByDescending(x => x.EntityType)
                        : q.OrderBy(x => x.EntityType);

                case SortOptions.FieldName:
                    return model.OrderByDescending
                        ? q.OrderByDescending(x => x.FieldName)
                        : q.OrderBy(x => x.FieldName);

                case SortOptions.CreationDate:
                    return model.OrderByDescending
                        ? q.OrderByDescending(x => x.CreationDate)
                        : q.OrderBy(x => x.CreationDate);

                default:
                    return q.OrderBy(x => x.Id);
            }
        },
        include: null,
        1,
        1000
        );


            var mappedConfigs = configList.Data
                .Select(x => new SystemConfigModel
                {
                    Id = x.Id,
                    FieldName = ConfigKeyDisplayNames.DisplayNames
                        .FirstOrDefault(kvp => kvp.Key.ToString() == x.FieldName).Value ?? x.FieldName!,
                    EntityType = x.EntityType.ToString(),
                    Value = x.Value,
                    EffectiveFrom = x.EffectiveFrom,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate
                }).ToList();

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                mappedConfigs = mappedConfigs
                    .Where(x => x.FieldName.Contains(model.Search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var pagedConfigs = mappedConfigs
           .Skip((model.PageIndex - 1) * model.PageSize)
           .Take(model.PageSize)
           .ToList();
            if (!pagedConfigs.Any())
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = "Configuration does not exist",
                    Data = null
                };
            }

            return new ResponseModel
            {
                Code = StatusCodes.Status200OK,
                Message = "Get Configurations Successfully",
                Data = new Pagination<SystemConfigModel>
                (
                    pagedConfigs,
                    model.PageIndex,
                    model.PageSize,
                    mappedConfigs.Count
                )
            };
        }

        public async Task<List<object[]>> GetAllAsKeyValueAsync()
        {
            var configs = await _unitOfWork.SystemConfigRepository.GetAllAsync();

            var distinctEntityTypes = configs.Data
                .Select(c => c.EntityType)
                .Distinct();

            return distinctEntityTypes
                .Select(entityType => new object[] { (int)entityType, entityType.ToString() })
                .ToList();
        }




        public async Task<ResponseModel> Get(SystemConfigKey key)
        {
            var config = await _unitOfWork.SystemConfigRepository.GetByKeyAsync(key);

            if (config == null)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = "Configuration does not exist",
                    Data = null
                };
            }
            string? value = config.Value?.Trim('"'); 

            var result = new SystemConfigModel
            {
                FieldName = config.FieldName!,
                EntityType = config.EntityType.ToString(),
                Value = value!,
                CreationDate = config.CreationDate,
                IsDeleted = config.IsDeleted,
                Id = config.Id,
                IsActive = config.IsActive,
                EffectiveFrom = config.EffectiveFrom
            };


            return new ResponseModel
            {
                Code = StatusCodes.Status200OK,
                Message = "Get Configuration Successfully",
                Data = result
            };
        }
    }
}
