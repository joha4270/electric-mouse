using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.HallViewModels;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.UserViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
    public class UserController: Controller
    {
        private ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult List()
        {
            IQueryable<ApplicationUser> users = _dbContext.Users;
            UserListViewModel model = new UserListViewModel { Users = users.ToList() };
            if (model.Users?.Count > 0)
            {
                return View(model);
            }
            return RedirectToAction(nameof(List), "Route");
        }

        public Task<IActionResult> Promote(UserListViewModel model)
        {
            var user = _dbContext.Users.First(h => h.ID == model.ID);

        }
    }
}
