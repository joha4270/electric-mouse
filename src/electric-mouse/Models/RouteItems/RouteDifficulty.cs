using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models.RouteItems
{
    [Table("Difficulties")]
    public class RouteDifficulty
    {
        [Key]
        [Required]
        public int RouteDifficultyID { get; set; }
        public string Name { get; set; }
    }
}
