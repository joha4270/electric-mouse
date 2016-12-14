using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using electric_mouse.Models;

namespace electric_mouse.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        bool IsSignedIn(ClaimsPrincipal principal);
        Task<ApplicationUser> FindByIdAsync(string userId);
    }
}