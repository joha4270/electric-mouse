using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace electric_mouse.Controllers
{
    public class RouteController : Controller
    {
        public IActionResult List()
        {
            // TODO: Make this a more viable solution
            ViewData["search"] = "1";
            return View();
        }

    }
}
