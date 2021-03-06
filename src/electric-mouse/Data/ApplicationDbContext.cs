﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using electric_mouse.Models;
using electric_mouse.Models.Relations;
using electric_mouse.Models.RouteItems;

namespace electric_mouse.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteHall> RouteHalls { get; set; }
        public DbSet<RouteSection> RouteSections { get; set; }
        public DbSet<RouteDifficulty> RouteDifficulties { get; set; }
        public DbSet<RouteSectionRelation> RouteSectionRelations { get; set; }
        public DbSet<RouteApplicationUserRelation> RouteUserRelations { get; set; }
        public DbSet<RouteAttachment> RouteAttachments { get; set; }
        public DbSet<AttachmentPathRelation> AttachmentPathRelations { get; set; } 
	    public DbSet<Comment> Comments { get; set; }

	    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<RouteApplicationUserRelation>()
                .HasKey(x => new {x.ApplicationUserRefId, x.RouteRefId});

            builder.Entity<RouteApplicationUserRelation>()
                .HasOne(rel => rel.User)
                .WithMany(u => u.RoutesCreated)
                .HasForeignKey(rel => rel.ApplicationUserRefId);

            builder.Entity<RouteApplicationUserRelation>()
                .HasOne(rel => rel.Route)
                .WithMany(u => u.Creators)
                .HasForeignKey(rel => rel.RouteRefId);

	        builder.Entity<Comment>()
		        .HasOne(rel => rel.User)
		        .WithMany(u => u.Comments)
		        .HasForeignKey(rel => rel.ApplicationUserRefId);
        }
    }
}
