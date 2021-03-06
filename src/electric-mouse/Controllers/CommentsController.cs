﻿using System;
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

namespace electric_mouse.Controllers
{
    public class CommentsController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;

		public CommentsController(IUserService userService, ICommentService commentService)
		{
            _commentService = commentService;
		    _userService = userService;
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = RoleHandler.Post)]
		public async Task<IActionResult> Reply(CommentViewModel model)
		{
			ApplicationUser user = await _userService.GetUserAsync(User);
            _commentService.AddComment(user, model.RouteID, model.CommentID, model.Content);

			return RedirectToAction(nameof(RouteController.Details), "Route", new { id = model.RouteID });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = RoleHandler.Post)]
		public async Task<IActionResult> AddComment(int routeID, string content)
		{
            ApplicationUser user = await _userService.GetUserAsync(User);
            _commentService.AddComment(user, routeID, 0, content);

			return RedirectToAction(nameof(RouteController.Details), "Route", new { id = routeID});
		}

	    [HttpPost]
	    [ValidateAntiForgeryToken]
	    public async Task<IActionResult> Delete(CommentViewModel model)
		{
			if (_userService.IsSignedIn(User))
			{
				ApplicationUser user = await _userService.GetUserAsync(User);
				bool userIsAdmin = await _userService.IsInRoleAsync(user, RoleHandler.Admin);
				bool userIsOwner = user.Id == model.ApplicationUserRefId;

				if (userIsOwner || userIsAdmin)
				{
					_commentService.DeleteComment(model.CommentID);
					return RedirectToAction(nameof(RouteController.Details), "Route", new { id = model.RouteID });
				}
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