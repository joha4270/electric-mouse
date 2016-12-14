using System.Collections.Generic;
using electric_mouse.Models.RouteItems;
using Models;

namespace electric_mouse.Models.HallViewModels
{
    public class HallCreateViewModel
    {
        public IList<RouteHall> Halls { get; set; }
        public IList<RouteSection> Sections { get; set; }
        public string Name { get; set; }
        public int? ID { get; set; }
        public RouteType Type { get; set; }
    }
}
