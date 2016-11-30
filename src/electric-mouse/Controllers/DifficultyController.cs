using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Data;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.DifficultyViewModels;

namespace electric_mouse.Controllers
{
    public class DifficultyController : Controller
    {
        private ApplicationDbContext _dbContext;

        public DifficultyController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
            if (!string.IsNullOrEmpty(model.Name))
            {
                _dbContext.RouteDifficulties.Add(new RouteDifficulty { Name = model.Name });
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Create), "Difficulty");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DifficultyCreateViewModel model)
        {
            RouteDifficulty diff = _dbContext.RouteDifficulties.Where(d => d.RouteDifficultyID == model.ID).First();

            _dbContext.RouteDifficulties.Remove(diff);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Create), "Difficulty");
        }
    }
}