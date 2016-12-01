using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Services
{
    public static class RoleHandler
    {
        public const string Admin = "Administrator";
        public const string Post = "Posting";

        public static void AddRoles(IServiceProvider service, params string[] roles)
        {
            RoleManager<IdentityRole> roleManager = service.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null) throw new InvalidOperationException($"No {typeof(RoleManager<IdentityRole>)} available. Did you setup roles?");
            
            foreach(string role in roles)
            {
                if (roleManager.RoleExistsAsync(role).Result) continue;

                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }
    }
}
