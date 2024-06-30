using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsApi.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Release_date",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Release_date",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }
    }
}
