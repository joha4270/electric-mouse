using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class routearchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Routes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Routes");
        }
    }
}
