using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplyRugby.Migrations
{
    /// <inheritdoc />
    public partial class SKillsComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kicking_DropComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kicking_GoalComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kicking_GrubberComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kicking_PuntComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Passing_PopComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passing_SpinComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passing_StandardComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tackling_FrontComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tackling_RearComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tackling_ScrambleComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tackling_SideComments",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kicking_DropComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Kicking_GoalComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Kicking_GrubberComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Kicking_PuntComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Passing_PopComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Passing_SpinComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Passing_StandardComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Tackling_FrontComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Tackling_RearComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Tackling_ScrambleComments",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Tackling_SideComments",
                table: "Skills");
        }
    }
}
