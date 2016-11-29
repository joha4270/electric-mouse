using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
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

        // RouteCreate name instead? - We'll have to implement hall etc create seperately
        public IActionResult Create()
        {
            RouteCreateViewModel model = new RouteCreateViewModel();
            model.Halls = _dbContext.RouteHalls.AsEnumerable();
            model.Difficulties = _dbContext.RouteDifficulties.AsEnumerable();
            model.Sections = model.Halls.ElementAt(0).Sections.AsEnumerable();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute(RouteCreateViewModel model)
        {
            IEnumerable<RouteHall> routeHalls = _dbContext.RouteHalls.AsEnumerable();
            IEnumerable<RouteDifficulty> difficulties = _dbContext.RouteDifficulties.AsEnumerable();
            IEnumerable<RouteSection> sections = _dbContext.RouteSections.AsEnumerable();

            int routeID = int.Parse(Request.Form["idNumber"]);
            RouteHall hall = routeHalls.ElementAt(int.Parse(Request.Form["hallSelect"]));
            RouteDifficulty difficulty = difficulties.ElementAt(int.Parse(Request.Form["difficultySelect"]));
            //TODO: Implement route type (Boulder/Sport)
            DateTime date = DateTime.Parse(Request.Form["datePicker"]);
            string gripColour = Request.Form["gripColor"];
            //TODO: Implement image and video data
            //TODO: Make it possible to add a route to more than one section
            //TODO: Make only the relevant sections visible (ie. only sections that are contained in the selected hall)
            RouteSection section = sections.ElementAt(int.Parse(Request.Form["sectionSelect"]));
            string note = Request.Form["noteText"];
            //TODO: Make exceptionhandling if the server could not parse the input (specially routeID could have problems)

            model.Route = new Route
            {
                Note = note,
                Difficulty = difficulty,
                RouteID = routeID,
                GripColour = gripColour,
                Date = date,
            };
            model.Hall = hall;

            model.Section = section;
            model.Section.Routes.Add(new RouteSectionRelation { RouteSection = model.Section, Route = model.Route });

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");
        }

        [HttpPost]
        public async Task<IActionResult> CreateSampleRoute(RouteCreateViewModel model)
        {
            model.Hall = new RouteHall { Name = "hall1" };
            _dbContext.RouteHalls.Add(model.Hall);
            model.Difficulty = new RouteDifficulty { Name = "Pink" };
            _dbContext.RouteDifficulties.Add(model.Difficulty);

            model.Section = new RouteSection { RouteHall = model.Hall, Name = "A" };

            model.Route = new Route { Note = "Plof", Difficulty = model.Difficulty, RouteID = 1, GripColour = "Black", Date = DateTime.Now };

            model.Section.Routes.Add(new RouteSectionRelation { RouteSection = model.Section, Route = model.Route });
            _dbContext.RouteSections.Add(model.Section);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");
        }

        public IActionResult List()
        {
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

            RouteListViewModel model = new RouteListViewModel { Routes = routeList };
            return View(model);
        }


    }
}
