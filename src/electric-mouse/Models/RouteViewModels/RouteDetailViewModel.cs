using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteDetailViewModel
    {
        public List<ApplicationUser> Creators { get; }
        public bool EditRights { get; }
        public List<CommentViewModel> Comments { get; }
        public RouteHall Hall { get; }
        public Route Routes { get; }
        public RouteSection Section { get; }

        public RouteDetailViewModel(Route routes, RouteSection section, RouteHall hall, List<CommentViewModel> root, List<ApplicationUser> creators, bool editRights)
        {
            Comments =root;
            Creators = creators;
            EditRights = editRights;
            Routes = routes;
            Section = section;
            Hall = hall;
        }
    }
}
