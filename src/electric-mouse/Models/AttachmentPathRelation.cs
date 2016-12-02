using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Models
{
    [Table("AttachmentPathRelations")]
    public class AttachmentPathRelation
    {
        [Key]
        [Required]
        public int AttachmentPathRelationID { get; set; }

        [ForeignKey("RouteAttachmentID")]
        public int RouteAttachmentID { get; set; }

        public virtual string ImagePath { get; set; }

        public virtual RouteAttachment RouteAttachment { get; set; }
    }


    //public class RouteSectionRelation
    //{
    //    [Key]
    //    public int RouteSectionRelationID { get; set; }

    //    [ForeignKey("RouteSectionID")]
    //    public int RouteSectionID { get; set; }

    //    [ForeignKey("ID")]
    //    public int RouteID { get; set; }

    //    public virtual Route Route { get; set; }

    //    public virtual RouteSection RouteSection { get; set; }
    //}
}
