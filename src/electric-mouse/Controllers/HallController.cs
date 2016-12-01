﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models.HallViewModels;
using electric_mouse.Models.RouteItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
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
            if(!string.IsNullOrEmpty(model.Name))
            {
                _dbContext.RouteHalls.Add(new RouteHall { Name = model.Name, Sections = new List<RouteSection>() });
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