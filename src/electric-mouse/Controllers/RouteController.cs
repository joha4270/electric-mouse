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
            

            RouteSection sec = new RouteSection {RouteHall = rh, Name = "A"};
            _dbContext.RouteSections.Add(sec);

            RouteDifficulty dif = new RouteDifficulty { Name = "Pink" };
            _dbContext.RouteDifficulties.Add(dif);

            Route r = new Route {Note = "Plof", Difficulty = dif, RouteID = 1, GripColour = "Black"};
            r.Sections.Add(new RouteSectionRelation {RouteSection = sec, Route = r});
            _dbContext.Routes.Add(r);

            _dbContext.SaveChanges();

            return "Din dejlige hello world hehe";
        }

        public IActionResult List()
        {
            // TODO: Make this a more viable solution
            ViewData["search"] = "1";

            IQueryable<Route> routes = _dbContext.Routes.Include(c => c.Difficulty);

            RouteListViewModel model = new RouteListViewModel {Routes = routes.ToList()};
            return View(model);
        }
        

    }
}
