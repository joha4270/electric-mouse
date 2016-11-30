﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace electric_mouse.Controllers
{
    public class RouteController : Controller
    {
        private ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public RouteController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILoggerFactory logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger.CreateLogger<RouteController>();
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
            //int routeID = int.Parse(Request.Form["idNumber"]);
            RouteDifficulty difficulty =
                _dbContext.RouteDifficulties.First(d => d.RouteDifficultyID == model.RouteDifficultyID);
            //TODO: Implement route type (Boulder/Sport)
            DateTime date = DateTime.ParseExact(model.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            //TODO: Implement image and video data
            //TODO: Make it possible to add a route to more than one section
            //TODO: Make only the relevant sections visible (ie. only sections that are contained in the selected hall)
            RouteSection section = _dbContext.RouteSections.Include(h => h.RouteHall).First(s => s.RouteSectionID == model.RouteSectionID);

            Route route = new Route
            {
                Note = model.Note,
                Difficulty = difficulty,
                RouteID = model.RouteID,
                GripColour = model.GripColor,
                Date = date
            };


            section.Routes.Add(new RouteSectionRelation { RouteSection = section, Route = route });


            _dbContext.RouteUserRelations.Add(new RouteApplicationUserRelation{User = await _userManager.GetUserAsync(User), Route = route});

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

        public async Task<IActionResult> Details(int id)
        {
            List<CommentViewModel> root = new List<CommentViewModel> {
                new CommentViewModel
                {
                    Content = "Min mor laver ikke burgere",
                },
                new CommentViewModel
                {
                    Content = "Pancakes",
                    Children =
                    {
                        new CommentViewModel {Content = "Med is!" },
                        new CommentViewModel {Content = "Med syltetøj" }
                    }
                }
            };



            var routes = _dbContext
                .Routes
                .Where(r => r.ID == id)
                .Include(x => x.Difficulty).ToList().First();
            RouteSection section = null;
            RouteHall hall = null;


            foreach (var s in _dbContext.RouteSectionRelations.Where(t => t.RouteID == routes.ID).ToList())
            {
                section = _dbContext.RouteSections.First(t => s.RouteSectionID == t.RouteSectionID);
                hall = _dbContext.RouteHalls.First(p => p.RouteHallID == section.RouteHallID);
                break;
            }

            List<ApplicationUser> creators = _dbContext.RouteUserRelations.Where(r => r.Route == routes).Select(r => r.User).ToList();
            bool creatorOrAdmin = false;

            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                creatorOrAdmin = creators.Contains(user) || (await _userManager.IsInRoleAsync(user, "Administrator")); //TODO const when merging
            }

            return PartialView(new RouteDetailViewModel(routes, section, hall, root, creators, creatorOrAdmin));

        }


        //TODO: UNCOMMENT [Authorize(Roles=RoleSetup.Post)]
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            //Cannot be null as Role requires user being logged in
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (
                await _dbContext.RouteUserRelations.AnyAsync(x => x.RouteRefId == id && x.ApplicationUserRefId == user.Id) ||
                await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                _logger.LogInformation("Deleting route with id = {id}", id);

                Route route = await _dbContext.Routes.FirstOrDefaultAsync<Route>(x => x.ID == id);
                route.Archived = true;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("List"); //TODO: return to earlier search
            }


            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return Content("You don't have access to this action. 403 Forbidden");
        }

        //TODO: UNCOMMENT [Authorize(Roles=RoleSetup.Post)]
        public async Task<IActionResult> Update(int id)
        {
            //Cannot be null as Role requires user being logged in
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (
                await _dbContext.RouteUserRelations.AnyAsync(x => x.RouteRefId == id && x.ApplicationUserRefId == user.Id) ||
                await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                _logger.LogInformation("Updating route with id = {id}",id);

                //todo: create route information with existing id
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return Content("You don't have access to this action. 403 Forbidden");
        }
    }
}
