using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.Api;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Controllers
{
    public class ApiController : Controller
    {
        private readonly IApiService _apiService;

        public ApiController(IApiService apiService)
        {
	        _apiService = apiService;
        }

        public async Task<IActionResult> QueryUsers(string query)
        {
	        List<UserSearchUserResultModel> model = _apiService.QueryUsers(query);

            return Json(model);
        }
    }
}