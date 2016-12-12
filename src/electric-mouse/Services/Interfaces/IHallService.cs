using System.Collections.Generic;
using electric_mouse.Models.RouteItems;
using Models;

namespace electric_mouse.Services.Interfaces
{
    public interface IHallService
    {
        void AddHall(string name, RouteType type);
        List<RouteHall> GetActiveHalls();
        void DeleteHall(int id);
    }
}