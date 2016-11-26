using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
    public class RouteController : Controller
    {
        private ApplicationDbContext _dbContext;

        public RouteController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // async
        public async Task<IActionResult> Create(RouteCreateViewModel model)
        {
            model.Hall = new RouteHall {Name = "hall1"};
            _dbContext.RouteHalls.Add(model.Hall);
            model.Difficulty = new RouteDifficulty { Name = "Pink" };
            _dbContext.RouteDifficulties.Add(model.Difficulty);
            
            model.Section = new RouteSection {RouteHall = model.Hall, Name = "A"};

            model.Route = new Route {Note = "Plof", Difficulty = model.Difficulty, RouteID = 1, GripColour = "Black", Date = DateTime.Now};

            model.Section.Routes.Add(new RouteSectionRelation { RouteSection = model.Section, Route = model.Route });
            _dbContext.RouteSections.Add(model.Section);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");

            //return "Sample Data Created";
        }

        public IActionResult List()
        {
            // TODO: Make this a more viable solution
            ViewData["search"] = "1";

            // Can probably be done using LINQ and lambda, this'll do though
            IQueryable<Route> routes = _dbContext.Routes.Include(c => c.Difficulty); // Using include for 'recursion'
            IList<Route> routeList = new List<Route>();

            foreach (var r in routes.ToList())
            {
                r.Sections = new List<RouteSection>();
                List<RouteSectionRelation> relations = _dbContext.RouteSectionRelations.Where(t => t.RouteID == r.ID).ToList();
                foreach (var s in relations)
                {
                    RouteSection section = _dbContext.RouteSections.First(t => s.RouteSectionID == t.RouteSectionID);
                    r.Sections.Add(section);
                }

                routeList.Add(r);
            }

            RouteListViewModel model = new RouteListViewModel {Routes = routeList};
            return View(model);
        }
        

    }
}
