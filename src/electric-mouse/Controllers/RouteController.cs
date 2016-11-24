using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private RouteContext _dbContext;

        public RouteController(RouteContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Create()
        {
            RouteHall rh = new RouteHall {Name = "hall1"};
            _dbContext.RouteHalls.Add(rh);
            RouteDifficulty dif = new RouteDifficulty { Name = "Pink" };
            _dbContext.RouteDifficulties.Add(dif);
            
            RouteSection sec = new RouteSection {RouteHall = rh, Name = "A"};

            Route r = new Route {Note = "Plof", Difficulty = dif, RouteID = 1, GripColour = "Black"};

            sec.Routes.Add(new RouteSectionRelation { RouteSection = sec, Route = r });
            _dbContext.RouteSections.Add(sec);

            _dbContext.SaveChanges();

            return "Din dejlige hello world hehe";
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
