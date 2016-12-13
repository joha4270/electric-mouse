using System.Collections.Generic;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Services.Interfaces
{
    public interface ISectionService
    {
        void AddSection(string name, int? routeHallId);
        void ArchiveAllRoutesInSection(int? sectionId);
        void ArchiveSection(int? sectionId);
        List<RouteHall> GetAllRouteHalls();
        List<RouteSection> GetAllRouteSections();
    }
}