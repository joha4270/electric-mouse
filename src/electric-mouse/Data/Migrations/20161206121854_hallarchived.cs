using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class hallarchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Halls",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Halls");
        }
    }
}
