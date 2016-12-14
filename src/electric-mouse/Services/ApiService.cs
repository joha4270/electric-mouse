using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.Api;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Services
{
	public class ApiService : IApiService
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public ApiService(UserManager<ApplicationUser> userManager)
		{
			//_dbContext = dbContext;
			_userManager = userManager;
		}

		// Query
		public List<UserSearchUserResultModel> QueryUsers(string query)
		{
			List<ApplicationUser> rawUser = _userManager.Users
				.Where(u => u.DisplayName.ToUpper().Contains(query.ToUpper()))
				.OrderBy(u => u.DisplayName)
				.Take(10)
				.ToList();

			List<UserSearchUserResultModel> censoredUser = rawUser.Select(UserSearchUserResultModel.FromApplicationUser).ToList();

			return censoredUser;
		}
	}
}