using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
    public partial class type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Routes",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddColumn<int>(
                name: "ExpectedType",
                table: "Halls",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Routes");
            migrationBuilder.DropColumn(
                name: "ExpectedType",
                table: "Halls");
        }
    }
}
