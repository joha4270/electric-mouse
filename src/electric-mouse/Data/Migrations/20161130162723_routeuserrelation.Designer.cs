using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using electric_mouse.Data;

namespace electric_mouse.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20161130162723_routeuserrelation")]
    partial class routeuserrelation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("electric_mouse.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("electric_mouse.Models.Route", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Date");

                    b.Property<string>("GripColour");

                    b.Property<string>("Note");

                    b.Property<int>("RouteDifficultyID");

                    b.Property<int>("RouteID");

                    b.HasKey("ID");

                    b.HasIndex("RouteDifficultyID");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("electric_mouse.Models.RouteApplicationUserRelation", b =>
                {
                    b.Property<string>("ApplicationUserRefId");

                    b.Property<int>("RouteRefId");

                    b.HasKey("ApplicationUserRefId", "RouteRefId");

                    b.HasIndex("ApplicationUserRefId");

                    b.HasIndex("RouteRefId");

                    b.ToTable("RouteUserRelations");
                });

            modelBuilder.Entity("electric_mouse.Models.RouteItems.RouteDifficulty", b =>
                {
                    b.Property<int>("RouteDifficultyID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("RouteDifficultyID");

                    b.ToTable("Difficulties");
                });

            modelBuilder.Entity("electric_mouse.Models.RouteItems.RouteHall", b =>
                {
                    b.Property<int>("RouteHallID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("RouteHallID");

                    b.ToTable("Halls");
                });

            modelBuilder.Entity("electric_mouse.Models.RouteItems.RouteSection", b =>
                {
                    b.Property<int>("RouteSectionID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("RouteHallID");

                    b.HasKey("RouteSectionID");

                    b.HasIndex("RouteHallID");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("electric_mouse.Models.RouteSectionRelation", b =>
                {
                    b.Property<int>("RouteSectionRelationID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RouteID");

                    b.Property<int>("RouteSectionID");

                    b.HasKey("RouteSectionRelationID");

                    b.HasIndex("RouteID");

                    b.HasIndex("RouteSectionID");

                    b.ToTable("RouteSectionRelations");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("electric_mouse.Models.Route", b =>
                {
                    b.HasOne("electric_mouse.Models.RouteItems.RouteDifficulty", "Difficulty")
                        .WithMany()
                        .HasForeignKey("RouteDifficultyID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("electric_mouse.Models.RouteApplicationUserRelation", b =>
                {
                    b.HasOne("electric_mouse.Models.ApplicationUser", "User")
                        .WithMany("RoutesCreated")
                        .HasForeignKey("ApplicationUserRefId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("electric_mouse.Models.Route", "Route")
                        .WithMany("Creators")
                        .HasForeignKey("RouteRefId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("electric_mouse.Models.RouteItems.RouteSection", b =>
                {
                    b.HasOne("electric_mouse.Models.RouteItems.RouteHall", "RouteHall")
                        .WithMany("Sections")
                        .HasForeignKey("RouteHallID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("electric_mouse.Models.RouteSectionRelation", b =>
                {
                    b.HasOne("electric_mouse.Models.Route", "Route")
                        .WithMany()
                        .HasForeignKey("RouteID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("electric_mouse.Models.RouteItems.RouteSection", "RouteSection")
                        .WithMany("Routes")
                        .HasForeignKey("RouteSectionID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("electric_mouse.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("electric_mouse.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("electric_mouse.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
