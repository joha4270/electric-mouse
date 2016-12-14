using System.Collections.Generic;
using electric_mouse.Models.Api;

namespace electric_mouse.Services.Interfaces
{
	public interface IApiService
	{
		List<UserSearchUserResultModel> QueryUsers(string query);
	}
}