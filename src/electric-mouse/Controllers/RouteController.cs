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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace electric_mouse.Controllers
{
    public class RouteController : Controller
    {
        private ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private IHostingEnvironment _environment;

        public RouteController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILoggerFactory logger, IHostingEnvironment environment)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger.CreateLogger<RouteController>();
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
            if (model.RouteSectionID != null)
            {
                foreach (var sectionId in model.RouteSectionID)
                {
                    RouteSection section = _dbContext.RouteSections.Include(h => h.RouteHall).First(s => s.RouteSectionID == sectionId);
                    section.Routes.Add(new RouteSectionRelation { RouteSection = section, Route = route });
                }

                _dbContext.SaveChanges();
            }


            // Add the video attachment to the database
            RouteAttachment attachment = new RouteAttachment { VideoUrl = model.VideoUrl, Route = route, RouteID = route.ID };
            _dbContext.RouteAttachments.Add(attachment);

            IFormFileCollection imgs = Request.Form.Files;
            // Add the image(s) attachment to the database
            AddImageAttachmentsToDatabase(imgs, attachment);

            _dbContext.RouteUserRelations.Add(new RouteApplicationUserRelation{User = await _userManager.GetUserAsync(User), Route = route});

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(List), "Route");
        }

        private async void AddImageAttachmentsToDatabase(IFormFileCollection images, RouteAttachment attachment)
        {
            if (images == null) // bail if there are no images being uploaded
                return;

            // the name of the folder to upload all the images
            string uploadFolderName = "uploads";
            // generate a random file name for all the images that are being uploaded
            string[] imageFileNames = images.Select(image => GetRandomFileNameWithOriginalExtension(image.FileName)).ToArray();
            // get all the relative paths (uploads\<filename>)
            string[] relativeImagePaths = imageFileNames.Select(filename => Path.Combine(uploadFolderName, filename)).ToArray();
            // get the path to the uploads folder on the server
            string uploadFolderPath = Path.Combine(_environment.WebRootPath, uploadFolderName);
            // get the full path (c:\...\wwwroot\uploads\<filename>)
            string[] fullImagePaths = imageFileNames.Select(filename => Path.Combine(uploadFolderPath, filename)).ToArray();
            int i = 0;

            // create uploads folder if it doesnt exist
            if(Directory.Exists(uploadFolderPath) == false)
                Directory.CreateDirectory(uploadFolderPath);

            foreach (IFormFile image in images)
            {
                if (image.Length < 0 && image.Length > ConvertMegabytesToBytes(5)) // image size should not exceed 5 megabytes
                    continue; // skip the iteration; dont upload the image

                if (image.ContentType.Contains("image") == false)
                    continue; // if it isnt an image being uploaded; skip it!

                if (HasExtension(image.FileName, ".png", ".jpg", ".jpeg") == false)
                    continue; // if it doesnt have an image extension; skip it!

                using (FileStream fileStream = new FileStream(fullImagePaths[i], FileMode.Create))
                {
                    _dbContext.AttachmentPathRelations.Add(new AttachmentPathRelation
                    {
                        ImagePath = relativeImagePaths[i],
                        RouteAttachment = attachment
                    });

                    await image.CopyToAsync(fileStream);
                }
                i++;
            }
        }

        /// <summary>
        /// Checks if the filename has one of the extensions provided.
        /// </summary>
        private bool HasExtension(string fileName, params string[] extensions)
        {
            string fileExtension = Path.GetExtension(fileName);

            return extensions.Any(extension => extension.Equals(fileExtension));
        }

        #region These should probably be moved (can be made extension methods)

        public long ConvertMegabytesToBytes(long megabytes) => megabytes * 1000L * 1000L;

        public string GetRandomFileNameWithOriginalExtension(string fileName) => $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{Path.GetExtension(fileName)}";

        #endregion

        public IActionResult List(string archived = "false", string creator = null)
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
                routes = routes.Include(x => x.Creators)
                    .Where(x => x.Creators.Any(c => c.ApplicationUserRefId == creator));
            }

            routes = routes .Include(c => c.Difficulty);
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

            // Get all the images related to the route
            AttachmentPathRelation[] attachments = _dbContext.AttachmentPathRelations.Include(relation => relation.RouteAttachment).ToArray();
            string[] imagePaths = attachments?.Where(attachment => attachment.RouteAttachment.RouteID == id)
                                             ?.Select(attachment => attachment.ImagePath)
                                             .ToArray();

            return PartialView(new RouteDetailViewModel(routes, section, hall, root, creators, creatorOrAdmin, imagePaths));

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

        //[Authorize(Roles = RoleHandler.Post)]
        [HttpPost]
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
