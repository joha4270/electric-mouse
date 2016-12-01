using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.UserViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace electric_mouse.Controllers
{
    //[Authorize(Roles = Services.RoleHandler.Admin)]
    public class UserController: Controller
    {
        private ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _manager;

        public UserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> manager)
        {
            _dbContext = dbContext;
            _manager = manager;
        }
        
        public IActionResult List()
        {
            IQueryable<ApplicationUser> users = _dbContext.Users;
            UserListViewModel model = new UserListViewModel { Users = users.ToList() };
            if (model.Users?.Count > 0)
            {
                return View(model);
            }
            return RedirectToAction(nameof(List), "Route");
        }

        public async Task<IActionResult> PromoteMember(UserListViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if (await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                var user = _dbContext.Users.First(h => h.Id == model.ID);
                await _manager.AddToRoleAsync(user, "Administrator");
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(List), "User");
        }

        public async Task<IActionResult> DemoteAdmin(UserListViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if (await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                var user = _dbContext.Users.First(h => h.Id == model.ID);
                await _manager.RemoveFromRoleAsync(user, "Administrator");
                _dbContext.SaveChanges();
                if (admin.Id == model.ID)
                {
                    return RedirectToAction(nameof(List), "Route");
                }
            }
            return RedirectToAction(nameof(List), "User");
        }

        public async Task<IActionResult> Unban(UserListViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if(await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                var user = _dbContext.Users.First(h => h.Id == model.ID);
                await _manager.AddToRoleAsync(user, "Posting");
                _dbContext.SaveChanges();
            }
            
            return RedirectToAction(nameof(List), "User");
        }

        public async Task<IActionResult> Ban(UserListViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if (await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                var user = _dbContext.Users.First(h => h.Id == model.ID);
                await _manager.RemoveFromRoleAsync(user, "Posting");
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(List), "User");
        }

        public async Task<IActionResult> Delete(UserListViewModel model)
        {
            ApplicationUser admin = await _manager.GetUserAsync(User);
            if (await _manager.IsInRoleAsync(admin, Services.RoleHandler.Admin))
            {
                
                var user = _dbContext.Users.First(h => h.Id == model.ID);
                await _manager.DeleteAsync(user);
                _dbContext.SaveChanges();
            }
            

            return RedirectToAction(nameof(List), "User");
        }
    }
}
