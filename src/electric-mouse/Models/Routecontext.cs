using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Models.RouteItems;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Models
{
    /// <summary>
    /// Creates database tables.
    /// </summary>
    public class RouteContext : DbContext
    {
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteHall> RouteHalls { get; set; }
        public DbSet<RouteSection> RouteSections { get; set; }
        public DbSet<RouteDifficulty> RouteDifficulties { get; set; }
    }
}
