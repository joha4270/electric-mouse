using System.Collections.Generic;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteCreateViewModel
    {
        public RouteHall Hall { get; set; }
        public IEnumerable<RouteHall> Halls { get; set; }
        public RouteDifficulty Difficulty { get; set; }
        public IEnumerable<RouteDifficulty> Difficulties { get; set; }
        public RouteSection Section { get; set; }
        public IEnumerable<RouteSection> Sections { get; set; }
        public Route Route { get; set; }
    }
}