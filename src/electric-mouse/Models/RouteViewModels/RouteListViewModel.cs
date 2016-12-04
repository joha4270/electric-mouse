using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteListViewModel : BaseViewModel
    {
        public IList<Route> Routes { get; set; }
        public IList<RouteDifficulty> Difficulities { get; set; }
    }
}
