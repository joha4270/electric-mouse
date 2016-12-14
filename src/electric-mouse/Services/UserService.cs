using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using electric_mouse.Models;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace electric_mouse.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return _userManager.IsInRoleAsync(user, role);
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return _signInManager.IsSignedIn(principal);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }
    }
}
