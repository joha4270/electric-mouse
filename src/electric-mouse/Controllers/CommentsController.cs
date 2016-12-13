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
using System.Security.Claims;
using electric_mouse.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace electric_mouse.Controllers
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return _userManager.IsInRoleAsync(user, role);
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return _signInManager.IsSignedIn(principal);
        }
    }

    public interface IUserService
    {
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        bool IsSignedIn(ClaimsPrincipal principal);
    }

    public class CommentsController : Controller
    {
        private IUserService _userService;
        private readonly ICommentService _commentService;

		public CommentsController (IUserService userService, ICommentService commentService)
		{
            _commentService = commentService;
		    _userService = userService;
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = RoleHandler.Post)]
		public async Task<IActionResult> Reply (CommentViewModel model)
		{
			ApplicationUser user = await _userService.GetUserAsync(User);
            _commentService.AddComment(user, model.RouteID, model.CommentID, model.Content);
			return RedirectToAction(nameof(RouteController.List), "Route");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = RoleHandler.Post)]
		public async Task<IActionResult> AddComment (int routeID, string content)
		{
            ApplicationUser user = await _userService.GetUserAsync(User);
            _commentService.AddComment(user, routeID, 0, content);
			return RedirectToAction(nameof(RouteController.List), "Route");
		}


		public async Task<IActionResult> Delete (CommentViewModel model)
		{
			ApplicationUser user = null;
			bool userIsAdmin = false;
			bool userIsOwner = false;
			if (_userService.IsSignedIn(User))
			{
				user = await _userService.GetUserAsync(User);
				userIsAdmin = await _userService.IsInRoleAsync(user, "Administrator");
				userIsOwner = user.Id == model.ApplicationUserRefId;
			}

			if (userIsOwner || userIsAdmin)
			{
                _commentService.DeleteComment(model.CommentID);
				return RedirectToAction(nameof(RouteController.Details), "Route", new { id = 1 });
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