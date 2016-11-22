using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace electric_mouse.Controllers
{
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult Change(string currentCountry)
        {
            if(ModelState.IsValid)
            {
                HttpContext.Response.Cookies.Append("lang", currentCountry);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}