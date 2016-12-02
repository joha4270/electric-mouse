using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.DifficultyViewModels;
using Microsoft.AspNetCore.Identity;

namespace electric_mouse.Controllers
{
    public class DifficultyController : Controller
    {
        private ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _manager;

        public DifficultyController(ApplicationDbContext dbContext, UserManager<ApplicationUser> manager)
        {
            _dbContext = dbContext;
            _manager = manager;
        }


        public IActionResult Create()
        {
            IQueryable<RouteDifficulty> difficulty = _dbContext.RouteDifficulties;
            DifficultyCreateViewModel model = new DifficultyCreateViewModel { Difficulties = difficulty.ToList() };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DifficultyCreateViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if (await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    _dbContext.RouteDifficulties.Add(new RouteDifficulty { Name = model.Name, ColorHex = model.Color});
                    _dbContext.SaveChanges();
                }
            }
            
            return RedirectToAction(nameof(Create), "Difficulty");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DifficultyCreateViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if (await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                RouteDifficulty diff = _dbContext.RouteDifficulties.First(d => d.RouteDifficultyID == model.ID);

                _dbContext.RouteDifficulties.Remove(diff);
                _dbContext.SaveChanges();

            }
            
            return RedirectToAction(nameof(Create), "Difficulty");
        }
    }
}