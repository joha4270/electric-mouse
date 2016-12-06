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

        public RouteController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILoggerFactory logger
            ,IHostingEnvironment environment
            , AttachmentHandler attachmentHandler)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger.CreateLogger<RouteController>();
            _environment = environment;
            _attachmentHandler = attachmentHandler;
        }

        [Authorize(Roles = RoleHandler.Post)]
        // RouteCreate name instead? - We'll have to implement hall etc create seperately
        public async Task<IActionResult> Create()
        {

            IQueryable<RouteHall> routeHalls = _dbContext.RouteHalls.Include(s => s.Sections);
            IQueryable<RouteSection> routeSections = _dbContext.RouteSections;

            RouteCreateViewModel model = new RouteCreateViewModel();
            model.Halls = routeHalls.ToList();
            model.Difficulties = _dbContext.RouteDifficulties.ToList();
            model.Sections = routeSections.Where(s => !s.Archived).ToList();
            

            ApplicationUser user = await _userManager.GetUserAsync(User);
            model.Builders = new List<string>(){user.Id};
            model.BuilderList = new List<ApplicationUser>();
	        model.BuilderList.Add(user);

	        return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> Create(RouteCreateViewModel model)
        {
            _logger.LogInformation("Recived following users [{users}]", string.Join(", ", model.Builders));

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

            //Find all the users by id in model.Builders, in parallel, then discard missing results
            foreach (
                ApplicationUser builder in
                (await Task.WhenAll(
                    model.Builders
                        .Distinct()
                        .Select(x => _userManager.FindByIdAsync(x)))
                ).Where(x => x != null))
            {
                _dbContext.RouteUserRelations.Add(new RouteApplicationUserRelation{User = builder, Route = route});
            }


            // Add the video attachment to the database
            RouteAttachment attachment = new RouteAttachment { VideoUrl = model.VideoUrl, Route = route, RouteID = route.ID };
            _dbContext.RouteAttachments.Add(attachment);

            // Add the image(s) attachment and imagepaths to the database
            await UploadImages(attachment);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");
        }

        private async Task UploadImages(RouteAttachment attachment)
        {
            // Get all the image names that should be excluded from the upload
            string[] exclude = Request.Form["jfiler-items-exclude-Images-0"].ToString().Trim('[', ']').Replace("\"", "").Split(',');
            // Filter out the images that the user removed during the upload
            IEnumerable<IFormFile> imagesToUpload = Request.Form.Files.Where(image => image.Name.Contains("Images") && exclude.Contains(image.FileName) == false);
            string[] relativeImagePaths = await _attachmentHandler.SaveImagesOnServer(imagesToUpload.ToList(), _environment.WebRootPath, "uploads");

            foreach (string relativeImagePath in relativeImagePaths)
            {
                _dbContext.AttachmentPathRelations.Add(new AttachmentPathRelation
                {
                    ImagePath = relativeImagePath,
                    RouteAttachment = attachment
                });
            }
        }

        public async Task<IActionResult> List(string type = null, string archived = "false", string creator = null)
        {
            RouteType? nullableParsedtype = null;
            RouteType parsedtype;
            if (RouteType.TryParse(type, true, out parsedtype))
            {
                nullableParsedtype = parsedtype;
            }

            RouteListViewModel model = await GetListViewModel(archived, creator, nullableParsedtype);
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
                RouteListViewModel listModel = await GetListViewModel("false", null, model.Route.Type);

                listModel.ModalContent = new ModalContentViewModel
                {
	                ViewName = "Details",
	                Model = model
                };

                return View("List", listModel);
            }
        }

	    /// <summary>
	    /// Goes and fetches all information from database and loads it into a CommentViewModel, but not before recursively fetching all replies.
	    /// </summary>
	    private CommentViewModel FetchCommentData(Comment comment, List<Comment> AllComments, ApplicationUser User, bool UserIsAdmin)
	    {
		    List<Comment> replies = AllComments
			    .Where(x => x.OriginalPostID == comment.CommentID).ToList();
		    List<CommentViewModel> children = new List<CommentViewModel>();
		    foreach (Comment reply in replies)
		    {
			    children.Add(FetchCommentData(reply, AllComments, User, UserIsAdmin));
		    }
		    children.OrderByDescending<CommentViewModel, DateTime>(c => c.Date).Reverse();

		    bool userIsLoggedIn = User != null;
		    bool userIsOwner = userIsLoggedIn && comment.ApplicationUserRefId == User.Id;
		    bool deletionRights = userIsOwner || UserIsAdmin;
		    ApplicationUser user = _dbContext.Users.First(u => u.Id == comment.ApplicationUserRefId);
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

	    private async Task<RouteListViewModel> GetListViewModel(string archived, string creator, RouteType? type)
	    {
	        IQueryable<Route> routes = _dbContext.Routes;

            if (type.HasValue)
            {
                routes = routes.Where(r => r.Type == type);
            }

            //Build search query
            if (archived == "true")
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
            var difficulityList = _dbContext.RouteDifficulties.ToListAsync();
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

            return new RouteListViewModel { Routes = routeList, Difficulities = await difficulityList};
        }

        private async Task<RouteDetailViewModel> GetDetailViewModel(int id)
        {
            List<CommentViewModel> comments = new List<CommentViewModel>();

            Route route = _dbContext.Routes
                .Include(x => x.Difficulty)
	            .First(r => r.ID == id);

	        RouteSectionRelation rs = _dbContext.RouteSectionRelations.First(t => t.RouteID == route.ID);
	        RouteSection section = _dbContext.RouteSections.First(t => rs.RouteSectionID == t.RouteSectionID);
	        RouteHall hall = _dbContext.RouteHalls.First(p => p.RouteHallID == section.RouteHallID);

            List<ApplicationUser> creators = _dbContext.RouteUserRelations
	            .Where(r => r.Route == route)
	            .Select(r => r.User)
	            .ToList();

            bool creatorOrAdmin = false;

	        ApplicationUser user = null; // TODO: This makes no sense
	        bool userIsAdmin = false;
            if (_signInManager.IsSignedIn(User))
            {
                user = await _userManager.GetUserAsync(User);
	            userIsAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
	            creatorOrAdmin = creators.Contains(user) || (await _userManager.IsInRoleAsync(user, RoleHandler.Admin)); //TODO const when merging
            }

	        List<Comment> allComments = _dbContext.Comments
		        .Where(x => x.RouteID == id)
		        .ToList();
	        List<Comment> topLevelComments = allComments
		        .Where(x => x.OriginalPostID == 0)
		        .ToList();

	        foreach (Comment comment in topLevelComments)
	        {
		        comments.Add(FetchCommentData(comment, allComments, user, userIsAdmin));
	        }
	        comments = comments.OrderByDescending<CommentViewModel, DateTime>(c => c.Date).ToList();

            // Get all the images related to the route
            AttachmentPathRelation[] attachments = _dbContext.AttachmentPathRelations.Include(relation => relation.RouteAttachment)
                .Where(attachment => attachment.RouteAttachment.RouteID == id)
                .ToArray();
            string[] imagePaths = attachments?.Select(attachment => attachment.ImagePath)
                                             .ToArray();

            RouteDetailViewModel model = new RouteDetailViewModel
	        {
		        Route = route,
		        Section = section,
		        Hall = hall,
		        Creators = creators,
		        EditRights = creatorOrAdmin,
		        Comments = comments,
                Images = imagePaths,
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

        [Authorize(Roles = RoleHandler.Post)]
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
                List<ApplicationUser> Builders = await _dbContext
                    .RouteUserRelations
                    .Where(x => x.Route == route)
                    .Include(x => x.User)
                    .Select(x => x.User)
                    .ToListAsync();

                List<string> builderIDs = Builders.Select(x => x.Id).ToList();

                // Get all the images related to the route
                IList<AttachmentPathRelation> relations = _dbContext.AttachmentPathRelations
	                .Include(relation => relation.RouteAttachment)
	                .Where(x => x.RouteAttachment.RouteID == id)
	                .ToList();

                Tuple<string, int>[] imagePaths = relations?.Select(attachment => new Tuple<string, int>(attachment.ImagePath, attachment.AttachmentPathRelationID))
                                                 .ToArray();

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
                    RouteSectionID = selectedSections,
                    BuilderList = Builders,
                    Builders = builderIDs,
                    Images = imagePaths,
                    Attachment = _dbContext.RouteAttachments.FirstOrDefault(att => att.RouteID == id)
                };

                return View("Create", model);
            }
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

            return Content("You don't have access to this action. 403 Forbidden");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
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
                    List<int> toAddInt = model.RouteSectionID.Where(x => !inDb.Any(y => y.RouteSectionID == x)).ToList();
                    List<RouteSectionRelation> toAdd = toAddInt.Select(x => new RouteSectionRelation { Route = route, RouteSectionID = x }).ToList();


                    _dbContext.RouteSectionRelations.RemoveRange(toRemove);
                    _dbContext.RouteSectionRelations.AddRange(toAdd);
                }
                {
                    List<RouteApplicationUserRelation> inDb = await _dbContext.RouteUserRelations.Where(x => x.RouteRefId == model.UpdateID).ToListAsync();

                    List<RouteApplicationUserRelation> toRemove = inDb.Where(x => !model.Builders.Contains(x.ApplicationUserRefId)).ToList();
                    List<string> toAddId = model.Builders.Where(x => !inDb.Any(y => y.ApplicationUserRefId == x)).ToList();
                    List<RouteApplicationUserRelation> toAdd = toAddId.Select(x => new RouteApplicationUserRelation { Route = route, ApplicationUserRefId = x }).ToList();

                    _dbContext.RouteUserRelations.RemoveRange(toRemove);
                    _dbContext.RouteUserRelations.AddRange(toAdd);
                }

                #region Attachment related code
                // Get the path relations that should be deleted
                List<AttachmentPathRelation> pathRelationsToRemove = _dbContext.AttachmentPathRelations.Where(relation => model.ImagePathRelationID.Contains(relation.AttachmentPathRelationID)).ToList();
                // Get the paths to the images on the server that should be deleted
                List<string> imagesToDelete = pathRelationsToRemove.Select(relation => relation.ImagePath).ToList();

                // Delete the path relations
                _dbContext.AttachmentPathRelations.RemoveRange(pathRelationsToRemove);

                // Delete the images on the server
                foreach (string path in imagesToDelete)
                {
                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, path));
                }

                // If the video url is updated we want to update this in the attachment
                RouteAttachment attachment = _dbContext.RouteAttachments.First(att => att.RouteAttachmentID == model.AttachmentID);
                attachment.VideoUrl = model.VideoUrl;

                // Upload the new images to the server
                await UploadImages(attachment);
                #endregion

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("List");
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;

            return Content("You don't have access to this action. 403 Forbidden");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            ApplicationUser admin = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(admin, RoleHandler.Admin))
            {
                Route routeToDelete = _dbContext.Routes.First(route => route.ID == id);
                if (routeToDelete.Archived)
                    _dbContext.Routes.Remove(routeToDelete);

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(List), "Route", new { archived = "true" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeActive(int id)
        {
            ApplicationUser admin = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(admin, RoleHandler.Admin))
            {
                Route routeToMakeActive = _dbContext.Routes.First(route => route.ID == id);
                if (routeToMakeActive.Archived)
                {
                    routeToMakeActive.Archived = false;
                    _dbContext.Routes.Update(routeToMakeActive);
                    _dbContext.SaveChanges();
                }
            }

            return RedirectToAction(nameof(List), "Route", new { archived = "true" });
        }
    }
}
