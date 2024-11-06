using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class Migrations8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalNumber",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "PersonalNumber",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalNumber",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PersonalNumber",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
