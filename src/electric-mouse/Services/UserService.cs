using System.Threading.Tasks;
using electric_mouse.Models;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace electric_mouse.Services
{
	public class UserService : IUserService
	{
		private UserManager<ApplicationUser> _userManager;

		public UserService(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public ApplicationUser CreateUser(string username, string password)
		{
			var user = new ApplicationUser {UserName = username, Email = username};
			var result = _userManager.CreateAsync(user, password);

			return user;
		}
	}
}