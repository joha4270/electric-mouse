using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Data;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
    public class SectionController : Controller
    {
        private ApplicationDbContext _dbContext;

        public SectionController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SectionListViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SectionName) || !string.IsNullOrEmpty(model.Hall))
            {
                RouteSection section = new RouteSection { Name = model.SectionName, RouteHall = _dbContext.RouteHalls.First() };
                _dbContext.RouteSections.Add(section);
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(List), "Section");
        }

        public IActionResult List()
        {
            IList<RouteSection> sections = _dbContext.RouteSections.Include(s => s.RouteHall).Include(s => s.Routes).ToList();
            SectionListViewModel model = new SectionListViewModel {Sections = sections};
            model.Halls = _dbContext.RouteHalls.ToList();

            return View(model);
        }


    }
}
