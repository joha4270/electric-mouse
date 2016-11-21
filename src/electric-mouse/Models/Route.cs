using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models
{
    /// <summary>
    /// A route in the system.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// The key for linking to the database
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The ID the user sees.
        /// </summary>
        public int RouteID { get; set; }
        
        /// <summary>
    /// The hall the section is placed in.
    /// </summary>
        public RouteHall RouteHall { get; set; }

        /// <summary>
        /// The difficulty of the route.
        /// </summary>
        public RouteDifficulty RouteDifficulty { get; set; }

        /// <summary>
        /// The section the route is placed in.
        /// </summary>
        public RouteSection RouteSection { get; set; }

        /// <summary>
        /// A collection of the users who built the route.
        /// </summary>
        public ICollection<ApplicationUser> Builders { get; set; }
        
        /// <summary>
        /// The colour of the grips used in the route.
        /// </summary>
        public string GripColour { get; set; }

        /// <summary>
        /// The note given to the route.
        /// </summary>
        public string Note { get; set; }


    }
}
