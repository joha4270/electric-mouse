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
        [ValidateAntiForgeryToken]
        public IActionResult Change(string newLocale)
        {
            if(ModelState.IsValid)
            {
                HttpContext.Response.Cookies.Append("lang", newLocale);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}