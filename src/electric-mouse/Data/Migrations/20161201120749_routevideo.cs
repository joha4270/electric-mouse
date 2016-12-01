using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class routevideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RouteAttachments",
                columns: table => new
                {
                    RouteAttachmentID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ID = table.Column<int>(nullable: false),
                    VideoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteAttachments", x => x.RouteAttachmentID);
                    table.ForeignKey(
                        name: "FK_RouteAttachments_Routes_ID",
                        column: x => x.ID,
                        principalTable: "Routes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteAttachments_ID",
                table: "RouteAttachments",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteAttachments");
        }
    }
}
