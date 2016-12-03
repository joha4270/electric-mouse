using System;
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
using Microsoft.AspNetCore.Authorization;
using electric_mouse.Services;

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

        [Authorize(Roles= RoleHandler.Post)]
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
        [ValidateAntiForgeryToken]
        [Authorize(Roles= RoleHandler.Post)]
        public async Task<IActionResult> Create(RouteCreateViewModel model)
        {
            RouteDifficulty difficulty =
                _dbContext.RouteDifficulties.First(d => d.RouteDifficultyID == model.RouteDifficultyID);
            DateTime date = DateTime.ParseExact(model.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            //TODO: Implement image and video data
            
            // Create Route
            Route route = new Route
            {
                Note = model.Note,
                Difficulty = difficulty,
                RouteID = model.RouteID,
                GripColour = model.GripColor,
                Date = date,
                Type = model.Type
            };

            _dbContext.Routes.Add(route);
            _dbContext.SaveChanges();

            // Create Section Relation
            if (model.RouteSectionID != null)
            {
                foreach (var sectionId in model.RouteSectionID)
                {
                    RouteSection section = _dbContext.RouteSections.Include(h => h.RouteHall).First(s => s.RouteSectionID == sectionId);
                    section.Routes.Add(new RouteSectionRelation { RouteSection = section, Route = route });
                }

                _dbContext.SaveChanges();
            }


            _dbContext.RouteUserRelations.Add(new RouteApplicationUserRelation{User = await _userManager.GetUserAsync(User), Route = route});

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");
        }

        public IActionResult List(string archived = "false", string creator = null)
        {
            RouteListViewModel model = GetListViewModel(archived, creator);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            RouteDetailViewModel model = await GetDetailViewModel(id);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(model);
            }
            else
            {
                RouteListViewModel listModel = GetListViewModel("false", null);

                listModel.ModalContent = new ModalContentViewModel
                {
	                ViewName = "Details",
	                Model = model
                };

                return View("List", listModel);
            }
        }

        private RouteListViewModel GetListViewModel(string archived, string creator)
        {
            IQueryable<Route> routes = _dbContext.Routes;

            //Build search query
            if (archived == "yes")
            {
                routes = routes.Where(r => r.Archived == true);
            }
            else if (archived == "both") { } //noop
            //Catch all case here as instead of if yes else if no ; as it makes it less likely to expose on accident
            else
            {
                routes = routes.Where(r => r.Archived == false);
            }

            if (!string.IsNullOrEmpty(creator))
            {
                routes = routes
                    .Include(x => x.Creators)
                    .Where(x => x.Creators
                        .Any(c => c.ApplicationUserRefId == creator)
                    );
            }

            routes = routes.Include(c => c.Difficulty).Include(r => r.Creators).ThenInclude(l => l.User);
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

            return new RouteListViewModel { Routes = routeList };
        }

        private async Task<RouteDetailViewModel> GetDetailViewModel(int id)
        {
            List<CommentViewModel> comments = new List<CommentViewModel>();

            Route route = _dbContext
                .Routes
                .Include(x => x.Difficulty)
	            .First(r => r.ID == id);

	        RouteSectionRelation rs = _dbContext.RouteSectionRelations.First(t => t.RouteID == route.ID);
	        RouteSection section = _dbContext.RouteSections.First(t => rs.RouteSectionID == t.RouteSectionID);
	        RouteHall hall = _dbContext.RouteHalls.First(p => p.RouteHallID == section.RouteHallID);

            List<ApplicationUser> creators = _dbContext.RouteUserRelations.Where(r => r.Route == route).Select(r => r.User).ToList();
            bool creatorOrAdmin = false;

            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                creatorOrAdmin = creators.Contains(user) || (await _userManager.IsInRoleAsync(user, RoleHandler.Admin)); //TODO const when merging
            }
	        RouteDetailViewModel model = new RouteDetailViewModel
	        {
		        Route = route,
		        Section = section,
		        Hall = hall,
		        Creators = creators,
		        EditRights = creatorOrAdmin,
		        Comments = comments
	        };

	        return model;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= RoleHandler.Post)]
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

        [ValidateAntiForgeryToken]
        [Authorize(Roles= RoleHandler.Post)]
        public async Task<IActionResult> Update(int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (
                await
                    _dbContext.RouteUserRelations.AnyAsync(x => x.RouteRefId == id && x.ApplicationUserRefId == user.Id) ||
                await _userManager.IsInRoleAsync(user, "Administrator"))
            {

                Route route = await _dbContext.Routes.FirstOrDefaultAsync(x => x.ID == id);
                IQueryable<RouteHall> routeHalls = _dbContext.RouteHalls.Include(s => s.Sections);
                IQueryable<RouteSection> routeSections = _dbContext.RouteSections;
                int hall = _dbContext
                    .RouteSectionRelations
                    .Where(rel => rel.RouteID == route.ID)
                    .Include(rel => rel.RouteSection)
                    .Select(x => x.RouteSection.RouteHallID)
                    .First();

                List<int> selectedSections = _dbContext.RouteSectionRelations.Where(x => x.RouteID == id).Select(x => x.RouteSectionID).ToList();

                RouteCreateViewModel model = new RouteCreateViewModel
                {
                    Halls = routeHalls.ToList(),
                    Difficulties = _dbContext.RouteDifficulties.ToList(),
                    Sections = routeSections.ToList(),
                    Date = route.Date.ToString("yyyy-MM-dd"),
                    GripColor = route.GripColour,
                    Note = route.Note,
                    RouteDifficultyID = route.RouteDifficultyID,
                    RouteHallID = hall,
                    RouteID = route.RouteID,
                    UpdateID = id,
                    RouteSectionID = selectedSections
                };

                return View("Create", model);
            }
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

            return Content("You don't have access to this action. 403 Forbidden");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= RoleHandler.Post)]
        public async Task<IActionResult> Update(RouteCreateViewModel model)
        {
            
            //Cannot be null as Role requires user being logged in
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (
                ModelState.IsValid && (
                await _dbContext.RouteUserRelations.AnyAsync(x => x.RouteRefId == model.UpdateID && x.ApplicationUserRefId == user.Id) ||
                await _userManager.IsInRoleAsync(user, "Administrator")))
            {
                Route route = await _dbContext.Routes.FirstOrDefaultAsync(x => x.ID == model.UpdateID);

                route.RouteID = model.RouteID;
                route.Note = model.Note;
                route.GripColour = model.GripColor;
                route.RouteDifficultyID = model.RouteDifficultyID;
                route.Date = DateTime.ParseExact(model.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                {
                    List<RouteSectionRelation> inDb = await _dbContext.RouteSectionRelations.Where(x => x.RouteID == model.UpdateID).ToListAsync();

                    List<RouteSectionRelation> toRemove = inDb.Where(x => !model.RouteSectionID.Contains(x.RouteSectionID)).ToList();
                    List<RouteSectionRelation> toKeep = inDb.Where(x => model.RouteSectionID.Contains(x.RouteSectionID)).ToList(); ;
                    List<int> toAddInt = model.RouteSectionID.Where(x => !inDb.Any(y => y.RouteSectionID == x)).ToList();
                    List<RouteSectionRelation> toAdd = toAddInt.Select(x => new RouteSectionRelation { Route = route, RouteSectionID = x }).ToList();


                    _dbContext.RouteSectionRelations.RemoveRange(toRemove);
                    _dbContext.RouteSectionRelations.AddRange(toAdd);
                }
                //{
                //    //TODO: once we got users in model we need to fix this code. Read only code i'm afraid, shout at johannes
                //    List<RouteApplicationUserRelation> inDb = _dbContext.RouteUserRelations.Where(x => x.RouteRefId == model.UpdateID).ToList();
                //    List<RouteApplicationUserRelation> toRemove = inDb.Where(x => !route.Sections.Contains(x.RouteSection)).ToList();
                //    List<RouteApplicationUserRelation> toKeep = inDb.Where(x => route.Sections.Contains(x.RouteSection)).ToList(); ;
                //    List<RouteApplicationUserRelation> toAdd = model.Sections.Where(x => !inDb.Any(y => y.RouteSection == x)).Select(r => new RouteSectionRelation { Route = route, RouteSection = r }).ToList();

                //    _dbContext.RouteUserRelations.RemoveRange(toRemove);
                //    _dbContext.RouteUserRelations.AddRange(toAdd);
                //}

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("List");
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

            return Content("You don't have access to this action. 403 Forbidden");
        }
    }
}
