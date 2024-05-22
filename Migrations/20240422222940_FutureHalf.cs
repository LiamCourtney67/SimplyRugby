using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplyRugby.Migrations
{
    /// <inheritdoc />
    public partial class FutureHalf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstHalf_DatePlayed",
                table: "Matches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SecondHalf_DatePlayed",
                table: "Matches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstHalf_DatePlayed",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SecondHalf_DatePlayed",
                table: "Matches");
        }
    }
}
