using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteCreateViewModel
    {
        public RouteHall Hall { get; set; }
        public RouteDifficulty Difficulty { get; set; }
        public RouteSection Section { get; set; }
        public Route Route { get; set; }

    }
}