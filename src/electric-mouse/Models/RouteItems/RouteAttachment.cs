using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models.RouteItems
{
    public class RouteAttachment
    {
        [Key]
        [Required]
        public int RouteAttachmentID { get; set; }

        // this is the route id
        public int ID { get; set; }

        [ForeignKey("ID")]
        public Route Route { get; set; }
        
        public string VideoUrl { get; set; }
    }
}
