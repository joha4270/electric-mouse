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
    public class RouteContext// : DbContext
    {
        //public RouteContext() {}
        /*public RouteContext(DbContextOptions options) : base(options)
        {
            
        }*/

        
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Route section relation
            modelBuilder.Entity<RouteSectionRelation>()
                .HasKey(t => new { t.RouteID, t.RouteSectionID });

            modelBuilder.Entity<RouteSectionRelation>()
                .HasOne(pt => pt.Route)
                .WithMany(p => p.Sections)
                .HasForeignKey(pt => pt.RouteID);

            modelBuilder.Entity<RouteSectionRelation>()
                .HasOne(pt => pt.RouteSection)
                .WithMany(t => t.Routes)
                .HasForeignKey(pt => pt.RouteSectionID);
        }*/

        /*public DbSet<Route> Routes { get; set; }
        public DbSet<RouteHall> RouteHalls { get; set; }
        public DbSet<RouteSection> RouteSections { get; set; }
        public DbSet<RouteDifficulty> RouteDifficulties { get; set; }
        public DbSet<RouteSectionRelation> RouteSectionRelations { get; set; }*/
    }
}
