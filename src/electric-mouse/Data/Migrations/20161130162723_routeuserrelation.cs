using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class routeuserrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RouteUserRelations",
                columns: table => new
                {
                    ApplicationUserRefId = table.Column<string>(nullable: false),
                    RouteRefId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteUserRelations", x => new { x.ApplicationUserRefId, x.RouteRefId });
                    table.ForeignKey(
                        name: "FK_RouteUserRelations_AspNetUsers_ApplicationUserRefId",
                        column: x => x.ApplicationUserRefId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteUserRelations_Routes_RouteRefId",
                        column: x => x.RouteRefId,
                        principalTable: "Routes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteUserRelations_ApplicationUserRefId",
                table: "RouteUserRelations",
                column: "ApplicationUserRefId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteUserRelations_RouteRefId",
                table: "RouteUserRelations",
                column: "RouteRefId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteUserRelations");
        }
    }
}
