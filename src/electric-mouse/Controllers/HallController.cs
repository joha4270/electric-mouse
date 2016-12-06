using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models.HallViewModels;
using electric_mouse.Models.RouteItems;
using electric_mouse.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
    [Authorize(Roles= RoleHandler.Admin)]
    public class HallController : Controller
    {
        private ApplicationDbContext _dbContext;

        public HallController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Create()
        {
            IQueryable<RouteHall> halls = _dbContext.RouteHalls.Include(s => s.Sections);
            HallCreateViewModel model = new HallCreateViewModel { Halls = halls.ToList() };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HallCreateViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                model.Type--;
                RouteHall hall = new RouteHall
                {
                    Name = model.Name,
                    Sections = new List<RouteSection>()
                };

                if (model.Type >= 0)
                {
                    hall.ExpectedType = model.Type;
                }
                _dbContext.RouteHalls.Add(hall);
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Create), "Hall");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(HallCreateViewModel model)
        {
            var hall = _dbContext.RouteHalls.Include(s => s.Sections).First(h => h.RouteHallID == model.ID);

            if (hall.Sections?.Count <= 0)
            {
                _dbContext.RouteHalls.Remove(hall);
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Create), "Hall");
        }

    }
}
