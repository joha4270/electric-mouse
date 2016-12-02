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
}
