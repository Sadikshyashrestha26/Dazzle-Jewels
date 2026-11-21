using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dazzle_Jewels.Migrations
{
    /// <inheritdoc />
    public partial class AddPageView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagePath = table.Column<string>(type: "TEXT", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageViews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageViews");
        }
    }
}
