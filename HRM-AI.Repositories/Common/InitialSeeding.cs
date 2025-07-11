using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using OpenAI.GPT3.ObjectModels.ResponseModels;

namespace HRM_AI.Repositories.Common
{
    public static class InitialSeeding
    {
        private static readonly List<Entities.Role> Roles = new()
    {
        new() { Id = Guid.Parse("0195a73a-98fe-71a2-a007-25cc6c3d26f1"), Name = Enums.Role.GeneralManager.ToString() },
        new() { Id = Guid.Parse("0195a73a-9919-714b-a9ad-29b849e946f7"), Name = Enums.Role.Manager.ToString() },
        new() { Id = Guid.Parse("0195a73a-991d-70f0-aca4-78e00ff7557b"), Name = Enums.Role.Employee.ToString() }
    };
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();

         
            // Seed Roles
            foreach (var role in Roles)
            {
                if (!context.Roles.Any(r => r.Id == role.Id && r.Name == role.Name))
                {
                    role.CreationDate = DateTime.UtcNow;
                    context.Roles.Add(role);
                }
            }

        


         

            await context.SaveChangesAsync();
        }
    }

}

