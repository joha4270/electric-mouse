using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Data;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.EntityFrameworkCore;
using electric_mouse.Models;
using electric_mouse.Models.Relations;
using electric_mouse.Services;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace electric_mouse.Controllers
{
    [Authorize(Roles= RoleHandler.Admin)]
    public class SectionController : Controller
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SectionListViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SectionName) && model.HallID != null)
            {
                _sectionService.AddSection(model.SectionName, model.HallID);
            }
            
            return RedirectToAction(nameof(List), "Section");
        }

        public IActionResult List()
        {
            SectionListViewModel model = new SectionListViewModel
            {
                Sections = _sectionService.GetAllRouteSections(),
                Halls = _sectionService.GetAllRouteHalls()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(SectionListViewModel model)
        {
            _sectionService.ArchiveSection(model.SectionID);

            return RedirectToAction(nameof(List), "Section");
        }

        [HttpPost]
        public async Task<IActionResult> Clear(SectionListViewModel model)
        {
            _sectionService.ArchiveAllRoutesInSection(model.SectionID);

            return RedirectToAction(nameof(List), "Section");
        }
    }
}
