using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Models;
using Microsoft.AspNetCore.Authorization;
using electric_mouse.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using electric_mouse.Services;
using System.Net;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace electric_mouse.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public CommentsController (ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILoggerFactory logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger.CreateLogger<CommentsController>();
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> Reply (CommentViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            _dbContext.Comments.Add(new Comment
            {
                RouteID = model.RouteID,
                OriginalPostID = model.CommentID,
                User = user,
                Date = DateTime.Now,
                Content = model.Content
            });
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(RouteController.List), "Route");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleHandler.Post)]
        public async Task<IActionResult> AddComment (int RouteID, string Content)
        {
            CommentViewModel result = new CommentViewModel
            {
                RouteID = RouteID,
                Content = Content
            };
            await Reply(result);

            return RedirectToAction(nameof(RouteController.List), "Route");
        }


        public async Task<IActionResult> Delete (CommentViewModel model)
        {
            ApplicationUser user = null;
            bool userIsAdmin = false;
            bool userIsOwner = false;
            if (_signInManager.IsSignedIn(User))
            {
                user = await _userManager.GetUserAsync(User);
                userIsAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
                userIsOwner = user.Id == model.ApplicationUserRefId;
            }

            if (userIsOwner || userIsAdmin)
            {
                Comment comment = _dbContext.Comments.First(c => c.CommentID == model.CommentID);
                comment.Deleted = true;
                comment.DeletionDate = DateTime.Now;
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(RouteController.List), "Route");
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return Content("You don't have access to this action. 403 Forbidden");
        }


        /*public async Task<IActionResult> Edit (CommentViewModel model)
        {
            ApplicationUser user = null;
            bool userIsOwner = false;
            if (_signInManager.IsSignedIn(User))
            {
                user = await _userManager.GetUserAsync(User);
                userIsOwner = user.Id == model.User.Id;
            }

            if (userIsOwner)
            {
                _dbContext.Comments.First(c => c.CommentID == model.CommentID).Content = model.Content;
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(List), "Route");
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return Content("You don't have access to this action. 403 Forbidden");
        }*/

    }
}
