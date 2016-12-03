using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> QueryUsers(string query)
        {
            List<ApplicationUser> rawUser =
                _dbContext.Users
                .Where(u => u.DisplayName.Contains(query))
                .OrderBy(u => u.DisplayName)
                .Take(10)
                .ToList();

            List<UserSearchUserResultModel> censoredUser = rawUser.Select(UserSearchUserResultModel.FromApplicationUser).ToList();

            return Json(censoredUser);
        }
    }
}