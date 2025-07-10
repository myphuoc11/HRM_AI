using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Features;
using CloudinaryDotNet;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Models.AccountModels;
using HRM_AI.Repositories.Models.AccountRoleModels;
using HRM_AI.Services.Models.AccountModels;
using StackExchange.Redis;
using Role = HRM_AI.Repositories.Enums.Role;

namespace HRM_AI.Services.Common
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AccountSignUpModel, Repositories.Entities.Account>();
            CreateMap<Repositories.Entities.Account, AccountModel>(); 
                //.ForMember(dest => dest.Roles,
                //    opt => opt.MapFrom(src =>
                //        src.AccountRoles.Select(accountRole => accountRole.Role.Name).Select(Enum.Parse<Role>)))
                //.ForMember(dest => dest.RoleNames,
                //    opt => opt.MapFrom(src => src.AccountRoles.Select(accountRole => accountRole.Role.Name)));
            CreateMap<Repositories.Entities.Account, AccountLiteModel>();
            CreateMap<AccountUpdateModel, Repositories.Entities.Account>();
            CreateMap<AccountRole, AccountRoleModel>()
           .ForMember(dest => dest.Role,
               opt => opt.MapFrom(src => Enum.Parse<Role>(src.Role.Name)))
           .ForMember(dest => dest.RoleName,
               opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}
