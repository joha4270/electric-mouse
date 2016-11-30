using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models
{
    /// <summary>
    /// A route in the system.
    /// </summary>
    [Table("Routes")]
    public class Route
    {

        /// <summary>
        /// The key for linking to the database
        /// </summary>
        [Key]
        [Required]
        public int ID { get; set; }

        /// <summary>
        /// The ID the user sees.
        /// </summary>
        public int RouteID { get; set; }

        // Hallproperty get section parent.

        /// <summary>
        /// The difficulty of the route.
        ///// </summary>

        public int RouteDifficultyID { get; set; }
        
        
        [ForeignKey("RouteDifficultyID")]
        public RouteDifficulty Difficulty { get; set; }


        /// <summary>
        /// The section the route is placed in.
        /// </summary>
        [NotMapped]
        public ICollection<RouteSection> Sections { get; set; }

        

        /// <summary>
        /// A collection of the users who built the route.
        /// </summary>
        //public virtual ICollection<ApplicationUser> Builders { get; set; }

        /// <summary>
        /// The colour of the grips used in the route.
        /// </summary>
        public string GripColour { get; set; }

        /// <summary>
        /// The note given to the route.
        /// </summary>
        public string Note { get; set; }

        public DateTime Date { get; set; }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="builders"></param>
        //public void AddBuilders(ICollection<ApplicationUser> builders)
        //{
        //    foreach (ApplicationUser builder in builders)
        //    {
        //        Builders?.Add(builder);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        //public void AddSection(RouteSection section)
        //{
        //    Sections?.Add(section);
        //}
        
    }
}
