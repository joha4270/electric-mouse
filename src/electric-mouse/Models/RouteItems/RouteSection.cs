using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models.RouteItems
{
    [Table("Sections")]
    public class RouteSection
    {
        /// <summary>
        /// Section wich contains one or more routes
        /// </summary>
        [Key]
        [Required]
        public int RouteSectionID { get; set; }

        public int RouteHallID { get; set; }

        public string Name { get; set; }

        [ForeignKey("RouteHallID")]
        public virtual RouteHall RouteHall { get; set; }

        public virtual ICollection<RouteSectionRelation> Routes { get; set; }

        public RouteSection()
        {
            Routes = new List<RouteSectionRelation>();
        }

    }
}
