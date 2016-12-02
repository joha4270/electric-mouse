using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electric_mouse.Models.RouteItems
{
    [Table("Difficulties")]
    public class RouteDifficulty
    {
        [Key]
        [Required]
        public int RouteDifficultyID { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
    }
}
