using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models.Relations
{
    public class RouteSectionRelation
    {
        [Key]
        public int RouteSectionRelationID { get; set; }

        [ForeignKey("RouteSectionID")]
        public int RouteSectionID { get; set; }

        [ForeignKey("ID")]
        public int RouteID { get; set; }
        
        public virtual Route Route { get; set; }
        
        public virtual RouteSection RouteSection { get; set; }
    }
}
