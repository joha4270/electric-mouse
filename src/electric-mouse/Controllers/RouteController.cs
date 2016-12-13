using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.Api;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using electric_mouse.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using electric_mouse.Models.Relations;
using Microsoft.AspNetCore.Http;
using Models;

namespace electric_mouse.Controllers
{
    public class RouteController : Controller
    {
        private ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _environment;
        private readonly AttachmentHandler _attachmentHandler;
        private readonly RouteService _routeService;

        public RouteController
            (
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ILoggerFactory logger,
            IHostingEnvironment environment, 
            AttachmentHandler attachmentHandler,
            RouteService routeService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger.CreateLogger<RouteController>();
            _environment = environment;
            _attachmentHandler = attachmentHandler;
            _routeService = routeService;
        }

        [Authorize(Roles = RoleHandler.Post)]
        // RouteCreate name instead? - We'll have to implement hall etc create seperately
        public async Task<IActionResult> Create()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            RouteCreateViewModel model = new RouteCreateViewModel
            {
                Halls           = _routeService.GetAllActiveRouteHalls(),
                Difficulties    = _routeService.GetAllRouteDifficulties(), 
                Sections        = _routeService.GetAllActiveRouteSections(),
                Builders        = new List<string>
                {
                    user.Id
                },
                BuilderList = new List<ApplicationUser> {user}
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> Create(RouteCreateViewModel model)
        {
            _logger.LogInformation("Received following users [{users}]", string.Join(", ", model.Builders));
            
            // Create Route
            Route route = new Route
            {
                Note = model.Note,
                RouteID = model.RouteID,
                GripColour = model.GripColor,
                Type = model.Type
            };
            _routeService.AddRoute(route, model.Date, model.RouteDifficultyID);

            // Create Section Relation
            _routeService.AddRouteToSections(route, model.RouteSectionID.ToList());
            
            //Find all the users by id in model.Builders, in parallel, then discard missing results
            IEnumerable<ApplicationUser> builders =
                (
                    await Task.WhenAll
                        (
                            model.Builders
                                .Distinct()
                                .Select(userId => _userManager.FindByIdAsync(userId))
                        )
                )
                .Where(user => user != null);

            _routeService.AddBuildersToRoute(route, builders.ToArray());
            
            // Add the image(s) and imagepaths to the database, and the videourl
            string[] relativeImagePaths = await UploadImages();
            _routeService.AddAttachment(route, model.VideoUrl, relativeImagePaths);

            return RedirectToAction(nameof(List), "Route");
        }

        private async Task<string[]> UploadImages()
        {
            // Get all the image names that should be excluded from the upload
            string[] exclude = Request.Form["jfiler-items-exclude-Images-0"].ToString()
                                                                            .Trim('[', ']')
                                                                            .Replace("\"", "")
                                                                            .Split(',');
            // Filter out the images that the user removed during the upload
            IEnumerable<IFormFile> imagesToUpload = Request.Form.Files.Where
                (
                    image => image.Name.Contains("Images")
                             && exclude.Contains(image.FileName) == false
                );
            return await _attachmentHandler.SaveImagesOnServer(imagesToUpload.ToList(), _environment.WebRootPath, "uploads");
        }

        public async Task<IActionResult> List(string type = null, bool archived = false, string creator = null)
        {
            RouteType? nullableParsedtype = null;
            RouteType parsedtype;
            if (RouteType.TryParse(type, true, out parsedtype))
                nullableParsedtype = parsedtype;

            RouteListViewModel model = await GetListViewModel(archived, creator, nullableParsedtype);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            RouteDetailViewModel model = await GetDetailViewModel(id);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(model);

            RouteListViewModel listModel = await GetListViewModel(false, null, model.Route.Type);

            listModel.ModalContent = new ModalContentViewModel
            {
	            ViewName = "Details",
	            Model = model
            };

            return View("List", listModel);
        }

        private async Task<RouteListViewModel> GetListViewModel(bool archived, string creator, RouteType? type)
        {
            List<Route> routes = _routeService.GetRoutesFiltered(archived, creator, type);
            // limit the length of the notes in list view
            routes = _routeService.TruncateRouteNotes(routes, 50);

            return new RouteListViewModel
            {
                Routes = routes,
                Difficulities = _routeService.GetAllRouteDifficulties()
            };
        }

        /// <summary>
        /// Goes and fetches all information from database and loads it into a CommentViewModel, but not before recursively fetching all replies.
        /// </summary>
        private CommentViewModel FetchCommentData(Comment comment, List<Comment> allComments, ApplicationUser User, bool UserIsAdmin)
	    {
		    List<Comment> replies = allComments
			    .Where(x => x.OriginalPostID == comment.CommentID).ToList();
		    List<CommentViewModel> children = new List<CommentViewModel>();
		    foreach (Comment reply in replies)
		    {
			    children.Add(FetchCommentData(reply, allComments, User, UserIsAdmin));
		    }
		    children.OrderByDescending<CommentViewModel, DateTime>(c => c.Date).Reverse();

		    bool userIsLoggedIn = User != null;
		    bool userIsOwner = userIsLoggedIn && comment.ApplicationUserRefId == User.Id;
		    bool deletionRights = userIsOwner || UserIsAdmin;
	        ApplicationUser user = _routeService.GetUserById(comment.ApplicationUserRefId);
		    CommentViewModel result = new CommentViewModel
		    {
			    CommentID = comment.CommentID,
			    Deleted = comment.Deleted,
			    User = user,
			    ApplicationUserRefId = user.Id,
			    RouteID = comment.RouteID,
			    Date = comment.Date,
			    DeletionDate = comment.DeletionDate,
			    Content = comment.Content,
			    Children = children,
			    UserIsLoggedIn = userIsLoggedIn,
			    EditRights = userIsOwner,
			    DeletionRights = deletionRights
		    };

		    return result;
	    }

        private async Task<RouteDetailViewModel> GetDetailViewModel(int id)
        {
            List<CommentViewModel> comments = new List<CommentViewModel>();

            Route route = _routeService.GetRouteWithDifficultyById(id);

            RouteSection section = _routeService.GetRouteSectionThatRouteIsIn(id);
            RouteHall hall = _routeService.GetRouteHallById(section.RouteHallID);

            List<ApplicationUser> creators = _routeService.GetRouteCreators(id);

            bool creatorOrAdmin = false;

	        ApplicationUser user = null; // TODO: This makes no sense
	        bool userIsAdmin = false;
            if (_signInManager.IsSignedIn(User))
            {
                user = await _userManager.GetUserAsync(User);
	            userIsAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
	            creatorOrAdmin = creators.Contains(user) || (await _userManager.IsInRoleAsync(user, RoleHandler.Admin));
            }

            List<Comment> allCommentsInRoute = _routeService.GetCommentsInRoute(id);
            List<Comment> topLevelComments = allCommentsInRoute
                .Where(x => x.OriginalPostID == 0)
                .ToList();

	        foreach (Comment comment in topLevelComments)
	        {
		        comments.Add(FetchCommentData(comment, allCommentsInRoute, user, userIsAdmin));
	        }
	        comments = comments.OrderByDescending<CommentViewModel, DateTime>(c => c.Date).ToList();

            // Get all the images related to the route
            string[] imagePaths = _routeService.GetAllImagePathsInRoute(id);

            // Get the video url from the route attachment
            string url = _routeService.GetVideoUrlInRoute(id);

            RouteDetailViewModel model = new RouteDetailViewModel
            {
                Route = route,
                Section = section,
                Hall = hall,
                Creators = creators,
                EditRights = creatorOrAdmin,
                Comments = comments,
                Images = imagePaths,
                VideoUrl = url,
                UserIsLoggedIn = _signInManager.IsSignedIn(User) // TODO: This makes no sense
            };

	        return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> Archive(int id)
        {
            //Cannot be null as Role requires user being logged in
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (
                await _routeService.IsRouteCreatedByUser(id, user.Id) ||
                await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

                return Content("You don't have access to this action. 403 Forbidden");
            }
            _logger.LogInformation("Deleting route with id = {id}", id);

            _routeService.ArchiveRoute(id);

            return RedirectToAction("List"); //TODO: return to earlier search
        }

        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> Update(int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (!(await _routeService.IsRouteCreatedByUser(id, user.Id) ||
                  await _userManager.IsInRoleAsync(user, "Administrator")))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

                return Content("You don't have access to this action. 403 Forbidden");
            }
            
            Route route = await _routeService.GetRouteByIdAsync(id);
            List<ApplicationUser> builders = _routeService.GetRouteCreators(route.ID);
            List<string> builderIDs = builders.Select(u => u.Id).ToList();
            
            // Get all the images related to the route
            Tuple<string, int>[] imagePathsWithIds = _routeService.GetImagePathsWithIds(id);

            RouteCreateViewModel model = new RouteCreateViewModel
            {
                Halls = _routeService.GetAllActiveRouteHalls(),
                Difficulties = _routeService.GetAllRouteDifficulties(),
                Sections = _routeService.GetAllRouteSections(),
                Date = route.Date.ToString("yyyy-MM-dd"),
                GripColor = route.GripColour,
                Note = route.Note,
                RouteDifficultyID = route.RouteDifficultyID,
                RouteHallID = _routeService.GetRouteHallIdWhereThereRouteIs(route.ID),
                RouteID = route.RouteID,
                UpdateID = id,
                RouteSectionID = _routeService.GetRouteSectionsIdsWhereRouteIs(route.ID),
                BuilderList = builders,
                Builders = builderIDs,
                Images = imagePathsWithIds,
                Attachment = _routeService.GetRouteAttachmentInRoute(route.ID)
            };

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> Update(RouteCreateViewModel model)
        {
            //Cannot be null as Role requires user being logged in
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (
                !(ModelState.IsValid && (
                await _routeService.IsRouteCreatedByUser(model.UpdateID, user.Id) ||
                await _userManager.IsInRoleAsync(user, "Administrator"))))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                return Content("You don't have access to this action. 403 Forbidden");
            }
            
            Route route = _routeService.GetRouteById(model.UpdateID);
            route.RouteID = model.RouteID;
            route.Note = model.Note;
            route.GripColour = model.GripColor;
            route.RouteDifficultyID = model.RouteDifficultyID;
            route.Date = DateTime.ParseExact(model.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Remove and add sections (update)
            _routeService.RemoveSectionsFromRoute(model.UpdateID, model.RouteSectionID.ToArray());
            _routeService.AddSectionToRoute(model.UpdateID, model.RouteSectionID.ToArray());

            // Remove and add builders (update)
            _routeService.RemoveBuildersFromRoute(model.UpdateID, model.Builders.ToArray());
            _routeService.AddBuildersToRoute(model.UpdateID, model.Builders.ToArray());

            // Remove images if the user deleted them on website
            _routeService.RemoveImagesFromRoute(_environment.WebRootPath, model.ImagePathRelationID.ToArray());

            // If the video url is updated we want to update this in the attachment
            _routeService.UpdateVideoUrlInAttachment(model.AttachmentID, model.VideoUrl);

            // Upload the new images to the server
            string[] relativeImagePaths = await UploadImages();
            _routeService.UpdateImageAttachments(model.AttachmentID, relativeImagePaths);
            
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            ApplicationUser admin = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(admin, RoleHandler.Admin))
                _routeService.RemoveRoute(id);;

            return RedirectToAction(nameof(List), "Route", new { archived = "true" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeActive(int id)
        {
            ApplicationUser admin = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(admin, RoleHandler.Admin))
            {
                _routeService.ActivateRoute(id);
            }

            return RedirectToAction(nameof(List), "Route", new { archived = "true" });
        }
    }
}
