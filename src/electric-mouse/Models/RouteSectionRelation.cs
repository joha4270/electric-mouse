using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electric_mouse.Models
{
    public class RouteSectionRelation
    {
        
        public int RouteSectionID { get; set; }

        [ForeignKey("ID")]
        public int RouteID { get; set; }
        
        public virtual Route Route { get; set; }
        
        public virtual RouteSection RouteSection { get; set; }
    }
}
