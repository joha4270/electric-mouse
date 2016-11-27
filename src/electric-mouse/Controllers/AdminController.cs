using System.Security.Claims;
using System.Threading.Tasks;
using electric_mouse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace electric_mouse.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;


            string[] ensureRoles = new []{"Administrator"}; //Not sure if we want to do it like this, but we are probably going to require other roles. Posting rights, ect
            foreach (string roleName in ensureRoles)
            {
                if(_roleManager.RoleExistsAsync(roleName).Result) continue;

                _roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            }
        }

        public async Task<IActionResult> GetAdmin()
        {
            if (User.IsInRole("Administrator"))
            {
                ViewBag.Show = " You are already admin!";
            }
            else
            {
                _userManager.AddToRoleAsync(_userManager.GetUserAsync(User).Result, "Administrator").Wait();
                ViewBag.Show = "You are now admin";
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);

            ViewBag.Show += $"\n{user.Id} {user.NormalizedUserName}";

            return View("Display");
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Test()
        {
            ViewBag.Show = "You are admin";

            return View("Display");
        }
    }
}