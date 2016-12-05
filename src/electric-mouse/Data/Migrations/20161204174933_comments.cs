using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace electric_mouse.Data.Migrations
{
	public partial class comments : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Comments",
				columns: table => new
				{
					CommentID = table.Column<int>(nullable: false)
						.Annotation("Autoincrement", true),
					ApplicationUserRefId = table.Column<string>(nullable: true),
					Content = table.Column<string>(nullable: true),
					Date = table.Column<DateTime>(nullable: false),
					Deleted = table.Column<bool>(nullable: false),
					DeletionDate = table.Column<DateTime>(nullable: false),
					OriginalPostID = table.Column<int>(nullable: false),
					RouteID = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Comments", x => x.CommentID);
					table.ForeignKey(
						name: "FK_Comments_AspNetUsers_ApplicationUserRefId",
						column: x => x.ApplicationUserRefId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Comments_Routes_RouteID",
						column: x => x.RouteID,
						principalTable: "Routes",
						principalColumn: "ID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Comments_ApplicationUserRefId",
				table: "Comments",
				column: "ApplicationUserRefId");

			migrationBuilder.CreateIndex(
				name: "IX_Comments_RouteID",
				table: "Comments",
				column: "RouteID");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Comments");
		}
	}
}