using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteDetailViewModel
    {
        public List<CommentViewModel> Comments { get; }
        public RouteHall Hall { get; }
        public Route Routes { get; }
        public RouteSection Section { get; }
        
        public RouteDetailViewModel(Route routes, RouteSection section, RouteHall hall, List<CommentViewModel> root) 
        {
            Comments =root;
            Routes = routes;
            Section = section;
            Hall = hall;
        }
    }
}
