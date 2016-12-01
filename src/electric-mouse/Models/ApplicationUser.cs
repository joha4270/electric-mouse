using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace electric_mouse.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string FacebookID { get; set; }
        public string URLPath { get; set; }
        public string AuthToken { get; set; }
        public DateTime AuthTokenExpiration { get; set; }
    }
}
