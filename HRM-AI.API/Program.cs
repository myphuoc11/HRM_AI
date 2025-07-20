using System.Text.Json.Serialization;
using HRM_AI.API;
using HRM_AI.API.Middlewares;
using HRM_AI.Repositories.Common;
using HRM_AI.Services.Services;
using Microsoft.OpenApi.Models;
using OpenAI;
using OpenAI.GPT3.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Ignore all fields with null value in response
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

}).AddNewtonsoftJson(); 
builder.Services.AddHttpClient<ResumeParserAIService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ResumeParserAI:BaseUrl"]);
});
builder.Services.AddOpenAIService(settings =>
{
    settings.ApiKey = builder.Configuration["OpenAI:ApiKey"];
});


builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["JWT:ValidAudience"],
        Version = "v1"
    });
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    x.UseInlineDefinitionsForEnums();
    // Nếu cần, thêm: x.ResolveConflictingActions(api => api.First());
});


// Add API configuration
builder.Services.AddApiConfiguration(builder.Configuration);
// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();
// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();

// Allow CORS
app.UseCors("cors");
// Initial seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitialSeeding.Initialize(services);
}
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRM_AI API v1"));
//}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AccountStatusMiddleware>();

//app.UseSession();
//app.UseStaticFiles();

app.MapControllers();

app.Run();
