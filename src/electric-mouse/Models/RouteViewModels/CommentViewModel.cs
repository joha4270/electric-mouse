using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models
{
    public class CommentViewModel
    {
        public int CommentID { get; set; }
        public bool Deleted { get; set; }
        public ApplicationUser User { get; set; }
        public string ApplicationUserRefId { get; set; }
        public int RouteID { get; set; }
        public DateTime Date { get; set; }
        public DateTime DeletionDate { get; set; }
        public string Content { get; set; }
        public List<CommentViewModel> Children { get; set; } = new List<CommentViewModel>();
        public bool UserIsLoggedIn { get; set; }
        public bool EditRights { get; set; }
        public bool DeletionRights { get; set; }
        public CommentViewModel ()
        {

        }
    }
}
