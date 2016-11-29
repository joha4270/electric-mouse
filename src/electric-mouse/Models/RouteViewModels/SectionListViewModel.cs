using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class SectionListViewModel
    {
        public IList<RouteSection> Sections { get; set; }
        public IList<RouteHall> Halls { get; set; }
        public string SectionName { get; set; }
        public string Hall { get; set; }
    }
}
