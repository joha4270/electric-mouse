using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.DifficultyViewModels;
using electric_mouse.Services;
using Microsoft.AspNetCore.Authorization;
using electric_mouse.Services.Interfaces;
using System.Text.RegularExpressions;

namespace electric_mouse.Controllers
{
    [Authorize(Roles = RoleHandler.Admin)]
    public class DifficultyController : Controller
    {
        private readonly IDifficultyService _difficultyService;
        

        public DifficultyController(IDifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }


        public IActionResult Create()
        {
            DifficultyCreateViewModel model = new DifficultyCreateViewModel
            {
                Difficulties = _difficultyService.GetAllDifficulties()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DifficultyCreateViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Name) 
                && !string.IsNullOrEmpty(model.Color)
                && IsHexColorString(model.Color))
            {
                _difficultyService.AddDifficulty(model.Name, model.Color);
            }
            
            return RedirectToAction(nameof(Create), "Difficulty");
        }
        private bool IsHexColorString(string color)
        {
            if (Regex.IsMatch(color, @"^[a-fA-F0-9#]+$") && color.Length < 8)
                return true;
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DifficultyCreateViewModel model)
        {
            _difficultyService.RemoveDifficulty(model.ID);

            return RedirectToAction(nameof(Create), "Difficulty");
        }
    }
}