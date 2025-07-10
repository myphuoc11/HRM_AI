using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HRM_AI.Repositories.Common;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Enums;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.AccountModels;
using HRM_AI.Services.Models.TokenModels;
using HRM_AI.Services.Ultils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HRM_AI.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IClaimService _claimService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryHelper _cloudinaryHelper;
        private readonly IConfiguration _configuration;
        private readonly IEmailHelper _iIEmailHelper;
        private readonly IMapper _mapper;

        public AccountService(IClaimService claimService, IUnitOfWork unitOfWork, ICloudinaryHelper cloudinaryHelper, IConfiguration configuration, IEmailHelper iIEmailHelper, IMapper mapper)
        {
            _claimService = claimService;
            _unitOfWork = unitOfWork;
            _cloudinaryHelper = cloudinaryHelper;
            _configuration = configuration;
            _iIEmailHelper = iIEmailHelper;
            _mapper = mapper;
        }

        public Task<ResponseModel> AddRange(AccountAddRangeModel accountAddRangeModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> ChangePassword(AccountChangePasswordModel accountChangePasswordModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> ForgotPassword(AccountEmailModel accountEmailModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> Get(string idOrUsername)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAll(AccountFilterModel accountFilterModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAllAccount(AccountFilterModel accountFilterModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> RefreshToken(AccountRefreshTokenModel accountRefreshTokenModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> ResendVerificationEmail(AccountEmailModel accountEmailModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> ResetPassword(AccountResetPasswordModel accountResetPasswordModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> Restore(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> RevokeTokens(AccountEmailModel accountEmailModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> SendVerifyPhone(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> SignIn(AccountSignInModel accountSignInModel)
        {
            var account = await _unitOfWork.AccountRepository.FindByEmailAsync(accountSignInModel.Email);
            if (account != null)
            {
                if (account.IsDeleted)
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status410Gone,
                        Message = "Account has been deleted"
                    };

                if (AuthenticationTools.VerifyPassword(accountSignInModel.Password, account.HashedPassword))
                {
                    var tokenModel = await GenerateJwtToken(account);
                    if (tokenModel != null)
                        return new ResponseModel
                        {
                            Message = "Sign in successfully",
                            Data = tokenModel
                        };

                    return new ResponseModel
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Cannot sign in"
                    };
                }
            }

            return new ResponseModel
            {
                Code = StatusCodes.Status404NotFound,
                Message = "Invalid email or password"
            };
        }

        public Task<ResponseModel> SignInGoogle(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> SignUp(AccountSignUpModel accountSignUpModel)
        {
            var existedEmail = await _unitOfWork.AccountRepository.FindByEmailAsync(accountSignUpModel.Email);
            if (existedEmail != null)
                return new ResponseModel
                {
                    Code = StatusCodes.Status409Conflict,
                    Message = "Email already exists"
                };

            if (!string.IsNullOrWhiteSpace(accountSignUpModel.Username))
            {
                var existedUsername = await _unitOfWork.AccountRepository.FindByUsernameAsync(accountSignUpModel.Username);
                if (existedUsername != null)
                    return new ResponseModel
                    {
                        Code = StatusCodes.Status409Conflict,
                        Message = "Username already exists"
                    };
            }
            else
            {
                accountSignUpModel.Username = AuthenticationTools.GenerateUsername();
            }

            var account = _mapper.Map<Account>(accountSignUpModel);
            account.HashedPassword = AuthenticationTools.HashPassword(accountSignUpModel.Password);
            account.VerificationCode = AuthenticationTools.GenerateDigitCode(Constant.VerificationCodeLength);
            account.VerificationCodeExpiryTime = DateTime.UtcNow.AddMinutes(Constant.VerificationCodeValidityInMinutes);
           
            await _unitOfWork.AccountRepository.AddAsync(account);

            // Add "user" role as default
            var role = await _unitOfWork.RoleRepository.FindByNameAsync(Repositories.Enums.Role.Employee.ToString());
            var accountRole = new AccountRole
            {
                Account = account,
                Role = role!
            };
            await _unitOfWork.AccountRoleRepository.AddAsync(accountRole);
            if (await _unitOfWork.SaveChangeAsync() > 0)
            {
                // Email verification
                await SendVerificationEmail(account);

                return new ResponseModel
                {
                    Code = StatusCodes.Status201Created,
                    Message = "Sign up successfully, please verify your email"
                };
            }

            return new ResponseModel
            {
                Code = StatusCodes.Status500InternalServerError,
                Message = "Cannot sign up"
            };
        }

        public Task<ResponseModel> Update(Guid id, AccountUpdateModel accountUpdateModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> UpdateRoles(Guid id, AccountUpdateRolesModel accountUpdateRolesModel)
        {
            var account = await _unitOfWork.AccountRepository.GetAsync(id);
            if (account == null)
                return new ResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = "Account not found"
                };

            var existedAccountRoles =
                await _unitOfWork.AccountRoleRepository.GetAllAsync(accountRole => accountRole.AccountId == id,
                    include: accountRoles => accountRoles.Include(accountRole => accountRole.Role));
            var roles = await _unitOfWork.RoleRepository.GetAllAsync();
            var newRoles = roles.Data.Where(role =>
                accountUpdateRolesModel.Roles.Select(r => r.ToString()).Distinct().Contains(role.Name)).ToList();
            if (!newRoles.Any())
                return new ResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Invalid roles"
                };

            var rolesToRemove = existedAccountRoles.Data.Select(accountRole => accountRole.Role).Except(newRoles).ToList();
            // var rolesToAdd = newRoles.Except(existedAccountRoles.Data.Select(accountRole => accountRole.Role)).ToList();
            var rolesToAdd = newRoles;
            if (!rolesToRemove.Any() && !rolesToAdd.Any())
                return new ResponseModel
                {
                    Code = StatusCodes.Status409Conflict,
                    Message = "Account roles already exists"
                };

            // Remove roles
            var accountRolesToRemove = existedAccountRoles.Data
                .Where(accountRole => rolesToRemove.Contains(accountRole.Role)).ToList();
            foreach (var accountRole in accountRolesToRemove)
            {
                accountRole.Status = AccountStatus.Suspended;
                accountRole.IsDeleted = true;
            }

            if (rolesToRemove.Any()) _unitOfWork.AccountRoleRepository.UpdateRange(accountRolesToRemove);

            // Add roles
            if (rolesToAdd.Any())
            {
                var accountRolesToAdd = new List<AccountRole>();
                var accountRolesToRestore = new List<AccountRole>();
                foreach (var role in rolesToAdd)
                {
                    var existedAccountRole =
                        existedAccountRoles.Data.FirstOrDefault(accountRole => accountRole.Role.Id == role.Id);
                    if (existedAccountRole != null)
                    {
                        existedAccountRole.Status = AccountStatus.Active;
                        existedAccountRole.IsDeleted = false;
                        accountRolesToRestore.Add(existedAccountRole);
                    }
                    else
                    {
                        accountRolesToAdd.Add(new AccountRole
                        {
                            Account = account,
                            Role = role,
                            Status = AccountStatus.Active
                        });
                    }
                }

                _unitOfWork.AccountRoleRepository.UpdateRange(accountRolesToRestore);
                await _unitOfWork.AccountRoleRepository.AddRangeAsync(accountRolesToAdd);
            }

            if (await _unitOfWork.SaveChangeAsync() > 0)
            {
                return new ResponseModel
                {
                    Message = "Update account roles successfully"
                };
            }

            return new ResponseModel
            {
                Code = StatusCodes.Status500InternalServerError,
                Message = "Cannot update account roles"
            };
        }

        public Task<ResponseModel> VerifyEmail(string email, string verificationCode)
        {
            throw new NotImplementedException();
        }
        private async Task SendVerificationEmail(Account account)
        {
            await _iIEmailHelper.SendEmailAsync(account.Email, "Verify your email",
                $"Your verification code is {account.VerificationCode}. The code will expire in {Constant.VerificationCodeValidityInMinutes} minutes.",
                true);
        }
        private async Task<TokenModel?> GenerateJwtToken(Account account, RefreshToken? refreshToken = null,
        ClaimsPrincipal? principal = null)
        {
            // Refresh token information
            var authClaims = new List<Claim>();
            var deviceId = Guid.NewGuid();
            var refreshTokenString = Guid.NewGuid();
            DateTime expires = DateTime.UtcNow.AddDays(Constant.RefreshTokenValidityInDays);
            var roles = await _unitOfWork.RoleRepository.GetAllByAccountIdAsync(account.Id);

            // If refresh token then reuse the claims
            if (refreshToken != null && principal != null)
            {
                authClaims = principal.Claims
                    .Where(claim => claim.Type != ClaimTypes.Role && claim.Type != JwtRegisteredClaimNames.Aud).ToList();
                foreach (var role in roles) authClaims.Add(new Claim(ClaimTypes.Role, role.Name));
                refreshToken.Token = refreshTokenString;
                refreshToken.Expires = expires;
                deviceId = refreshToken.DeviceId;
                _unitOfWork.RefreshTokenRepository.Update(refreshToken);
            }

            // If sign in then add claims
            else
            {
                authClaims.Add(new Claim("accountId", account.Id.ToString()));
                authClaims.Add(new Claim("accountEmail", account.Email));
                authClaims.Add(new Claim("deviceId", deviceId.ToString()));
                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                foreach (var role in roles) authClaims.Add(new Claim(ClaimTypes.Role, role.Name));
                await _unitOfWork.RefreshTokenRepository.AddAsync(new RefreshToken
                {
                    DeviceId = deviceId,
                    Token = refreshTokenString,
                    Expires = expires,
                    CreatedBy = account
                });
            }

            if (await _unitOfWork.SaveChangeAsync() > 0)
            {
                var jwtToken = AuthenticationTools.CreateJwtToken(authClaims, _configuration);

                return new TokenModel
                {
                    DeviceId = deviceId,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    RefreshToken = refreshTokenString,
                    RefreshTokenExpires = expires,
                };
            }

            return null;
        }
    }
}
