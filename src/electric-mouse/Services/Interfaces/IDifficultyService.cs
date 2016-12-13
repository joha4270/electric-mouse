using electric_mouse.Models.RouteItems;
using System.Collections.Generic;

namespace electric_mouse.Services.Interfaces
{
    public interface IDifficultyService
    {
        List<RouteDifficulty> GetAllDifficulties();
        void AddDifficulty(string name, string color);
        void RemoveDifficulty(int? difficultyID);
    }
}