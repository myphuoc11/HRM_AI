using CloudinaryDotNet;
using HRM_AI.API.Middlewares;
using HRM_AI.API.Ultils;
using HRM_AI.Repositories.Common;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenAI.GPT3.Extensions;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using HRM_AI.Services.Common;
using HRM_AI.Repositories.Repositories;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Services;
using HRM_AI.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using OpenAI.GPT3.Extensions;

namespace HRM_AI.API
{
    public static class Configuration
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            #region Configuartion

            // Local database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("LocalDb")
                    ?? throw new InvalidOperationException("Connection string 'LocalDb' not found."));

            });



            // JWT
            var secret = configuration["JWT:Secret"];
            ArgumentException.ThrowIfNullOrWhiteSpace(secret);
            var issuer = configuration["JWT:ValidIssuer"];
            ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
            var audience = configuration["JWT:ValidAudience"];
            ArgumentException.ThrowIfNullOrWhiteSpace(audience);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrWhiteSpace(accessToken) && path.StartsWithSegments("/hub"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });              
            services.AddControllers()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            // CORS
            var clientUrl = configuration["URL:Client"];

            ArgumentException.ThrowIfNullOrWhiteSpace(clientUrl);

            var allowedOrigins = new[] { clientUrl };

            services.AddCors(options =>
            {
                options.AddPolicy("cors", corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            // OpenAI
            services.AddOpenAIService(settings =>
            {
                settings.ApiKey = configuration["OpenAI:ApiKey"]!;
            });
            //var clientUrls = configuration["URL:Client"]
            //    ?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            //    .ToArray();

            //if (clientUrls == null || !clientUrls.Any())
            //{
            //    throw new ArgumentException("Client URLs cannot be null or empty");
            //}

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("cors", policy =>
            //    {
            //        policy.WithOrigins(clientUrls)
            //              .AllowAnyHeader()
            //              .AllowAnyMethod()
            //              .AllowCredentials();
            //    });
            //});

            // Cloudinary
            var cloud = configuration["Cloudinary:Cloud"];
            ArgumentException.ThrowIfNullOrWhiteSpace(cloud);
            var apiKey = configuration["Cloudinary:ApiKey"];
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
            var cloudinary = new Cloudinary(new CloudinaryDotNet.Account { Cloud = cloud, ApiKey = apiKey, ApiSecret = apiSecret });
            services.AddSingleton<ICloudinary>(cloudinary);
            #endregion

            #region Middleware

            services.AddScoped<AccountStatusMiddleware>();
            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();

            #endregion

            #region Common

            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(MapperProfile).Assembly);
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion

            #region Helper

            services.AddScoped<ICloudinaryHelper, CloudinaryHelper>();
            services.AddTransient<IEmailHelper, EmailHelper>();
            //services.AddScoped<IRedisHelper, RedisHelper>();

            #endregion

            #region Dependency Injection

            // Account
            services.AddScoped<IAccountService, AccountService>();
            services.AddHttpClient<ResumeParserAIService>();
            services.AddSingleton<GoogleDriveService>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            // AccountRole
            services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();

            // Role
            services.AddScoped<IRoleRepository, RoleRepository>();

            // RefreshToken
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // OpenAi
            services.AddScoped<ICVParseService, CVParseService>();

            // Interviewer
            services.AddScoped<IInterviewerRepository, InterviewerRepository>();
            // InterviewSchedule
            services.AddScoped<IInterviewScheduleRepository, InterviewScheduleRepository>();
            // InterviewOutcome
            services.AddScoped<IInterviewOutcomeRepository, InterviewOutcomeRepository>();
            // CVApplicant
            services.AddScoped<ICVApplicantRepository, CVApplicantRepository>();
            // CVApplicantDetails
            services.AddScoped<ICVApplicantDetailsRepository, CVApplicantDetailsRepository>();
            // CampaignPosition
            services.AddScoped<ICampaignPositionRepository, CampaignPositionRepository>();
            // Campaign
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<ICampaignService, CampaignService>();
            // Department
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            // CampaignPositionDetail
            services.AddScoped<ICampaignPositionDetailRepository, CampaignPositionDetailRepository>();


            #endregion

            return services;
        }
    }
}
