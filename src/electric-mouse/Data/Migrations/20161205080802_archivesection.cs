using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class archivesection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Sections",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Sections");
        }
    }
}
