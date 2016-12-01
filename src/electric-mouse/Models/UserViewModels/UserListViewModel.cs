using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models.UserViewModels
{
    public class UserListViewModel
    {
        public IList<ApplicationUser> Users { get; set; }
        public string ID { get; set; }
    }
}
