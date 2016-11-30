using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models
{
    public class CommentViewModel
    {
        public string Content { get; set; }
        public List<CommentViewModel> Children { get; } = new List<CommentViewModel>();
    }
}
