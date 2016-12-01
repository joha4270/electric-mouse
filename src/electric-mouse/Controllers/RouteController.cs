using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace electric_mouse.Controllers
{
    public class RouteController : Controller
    {
        private ApplicationDbContext _dbContext;
        private IHostingEnvironment _environment;

        public RouteController(ApplicationDbContext dbContext, IHostingEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        // RouteCreate name instead? - We'll have to implement hall etc create seperately
        public IActionResult Create()
        {
            IQueryable<RouteHall> routeHalls = _dbContext.RouteHalls.Include(s => s.Sections);
            IQueryable<RouteSection> routeSections = _dbContext.RouteSections;

            RouteCreateViewModel model = new RouteCreateViewModel();
            model.Halls = routeHalls.ToList();
            model.Difficulties = _dbContext.RouteDifficulties.ToList();
            model.Sections = routeSections.ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RouteCreateViewModel model)
        {
            RouteDifficulty difficulty =
                _dbContext.RouteDifficulties.First(d => d.RouteDifficultyID == model.RouteDifficultyID);
            //TODO: Implement route type (Boulder/Sport)
            DateTime date = DateTime.ParseExact(model.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            //TODO: Implement image and video data
            //TODO: Make it possible to add a route to more than one section
            //TODO: Make only the relevant sections visible (ie. only sections that are contained in the selected hall)

            // Create Route
            Route route = new Route
            {
                Note = model.Note,
                Difficulty = difficulty,
                RouteID = model.RouteID,
                GripColour = model.GripColor,
                Date = date
            };

            _dbContext.Routes.Add(route);
            _dbContext.SaveChanges();

            // Create Section Relation
            foreach (var sectionId in model.RouteSectionID)
            {
                RouteSection section = _dbContext.RouteSections.Include(h => h.RouteHall).First(s => s.RouteSectionID == sectionId);
                section.Routes.Add(new RouteSectionRelation { RouteSection = section, Route = route });
            }

            // Add the video attachment to the database
            RouteAttachment attachment = new RouteAttachment { VideoUrl = model.VideoUrl, Route = route, ID = route.ID };
            _dbContext.RouteAttachments.Add(attachment);

            // Add the image(s) attachment to the database
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

            foreach (IFormFile image in model.Images)
            {
                using (var fileStream = new FileStream(Path.Combine(uploadPath, $"{DateTime.UtcNow.Ticks.ToString()}.{image.}"), FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");
        }



        [HttpPost]
        public async Task<IActionResult> CreateSampleRoute(RouteCreateViewModel model)
        {
            var Hall = new RouteHall { Name = "hall1" };
            _dbContext.RouteHalls.Add(Hall);
            var Difficulty = new RouteDifficulty { Name = "Pink" };
            _dbContext.RouteDifficulties.Add(Difficulty);

            var Section = new RouteSection { RouteHall = Hall, Name = "A" };

            var Route = new Route { Note = "Plof", Difficulty = Difficulty, RouteID = 1, GripColour = "Black", Date = DateTime.Now };

            Section.Routes.Add(new RouteSectionRelation { RouteSection = Section, Route = Route });
            _dbContext.RouteSections.Add(Section);

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
