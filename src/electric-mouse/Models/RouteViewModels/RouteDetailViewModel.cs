using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteDetailViewModel
    {
	    public RouteHall Hall { get; set;  }
	    public RouteSection Section { get; set; }
	    public Route Route { get; set;  }

	    public List<ApplicationUser> Creators { get; set;  }

        public List<CommentViewModel> Comments { get; set; }
	    public bool EditRights { get; set; }
    }
}
