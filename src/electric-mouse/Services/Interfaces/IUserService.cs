using electric_mouse.Models;

namespace electric_mouse.Services.Interfaces
{
	public interface IUserService
	{
		ApplicationUser CreateUser(string username, string password);
	}
}