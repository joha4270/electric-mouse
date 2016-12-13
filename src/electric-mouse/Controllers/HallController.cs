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
using electric_mouse.Services.Interfaces;

namespace electric_mouse.Controllers
{
    [Authorize(Roles= RoleHandler.Admin)]
    public class HallController : Controller
    {
        private ApplicationDbContext _dbContext;

        private IHallService _hallService;

        public HallController(IHallService hallService )
        {
            
            _hallService = hallService;
        }

        public IActionResult Create()
        {    
            HallCreateViewModel model = new HallCreateViewModel { Halls = _hallService.GetActiveHalls() };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HallCreateViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Name)&& ((int)model.Type<= 2))
            {
                _hallService.AddHall(model.Name, model.Type);
            }

            return RedirectToAction(nameof(Create), "Hall");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(HallCreateViewModel model)
        {
            if (model.ID!=null)
            {
                _hallService.DeleteHall(model.ID);
            }

            return RedirectToAction(nameof(Create), "Hall");
        }

    }
}
