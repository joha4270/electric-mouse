using System.Collections.Generic;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteCreateViewModel
    {
        public IList<RouteHall> Halls { get; set; }
        public IList<RouteDifficulty> Difficulties { get; set; }
        public IList<RouteSection> Sections { get; set; }

        public int RouteID { get; set; }
        public int RouteHallID { get; set; }
        public IList<int> RouteSectionID { get; set; }
        public int RouteDifficultyID { get; set; }
        public string Date { get; set; }
        public string GripColor { get; set; }
        public string Note { get; set; }

        public string VideoUrl { get; set; }
    }
}