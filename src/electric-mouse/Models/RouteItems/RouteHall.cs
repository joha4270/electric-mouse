using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace electric_mouse.Models.RouteItems
{/// <summary>
/// Hall contains one or more sections
/// </summary>
    [Table("Halls")]
    public class RouteHall
    {
        public RouteHall()
        {
            Sections = new List<RouteSection>();
        }

        [Key]
        [Required]
        public int RouteHallID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RouteSection> Sections { get; set; }

    }
}
