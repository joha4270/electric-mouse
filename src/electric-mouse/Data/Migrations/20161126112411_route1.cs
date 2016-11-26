using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class route1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Difficulties",
                columns: table => new
                {
                    RouteDifficultyID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Difficulties", x => x.RouteDifficultyID);
                });

            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    RouteHallID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.RouteHallID);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: true),
                    GripColour = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    RouteDifficultyID = table.Column<int>(nullable: false),
                    RouteID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Routes_Difficulties_RouteDifficultyID",
                        column: x => x.RouteDifficultyID,
                        principalTable: "Difficulties",
                        principalColumn: "RouteDifficultyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    RouteSectionID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    RouteHallID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.RouteSectionID);
                    table.ForeignKey(
                        name: "FK_Sections_Halls_RouteHallID",
                        column: x => x.RouteHallID,
                        principalTable: "Halls",
                        principalColumn: "RouteHallID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteSectionRelations",
                columns: table => new
                {
                    RouteSectionRelationID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    RouteID = table.Column<int>(nullable: false),
                    RouteSectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteSectionRelations", x => x.RouteSectionRelationID);
                    table.ForeignKey(
                        name: "FK_RouteSectionRelations_Routes_RouteID",
                        column: x => x.RouteID,
                        principalTable: "Routes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteSectionRelations_Sections_RouteSectionID",
                        column: x => x.RouteSectionID,
                        principalTable: "Sections",
                        principalColumn: "RouteSectionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteDifficultyID",
                table: "Routes",
                column: "RouteDifficultyID");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_RouteHallID",
                table: "Sections",
                column: "RouteHallID");

            migrationBuilder.CreateIndex(
                name: "IX_RouteSectionRelations_RouteID",
                table: "RouteSectionRelations",
                column: "RouteID");

            migrationBuilder.CreateIndex(
                name: "IX_RouteSectionRelations_RouteSectionID",
                table: "RouteSectionRelations",
                column: "RouteSectionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteSectionRelations");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Difficulties");

            migrationBuilder.DropTable(
                name: "Halls");
        }
    }
}
