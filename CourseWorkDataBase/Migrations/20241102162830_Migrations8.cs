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
                name: "ContactInfo",
                table: "Patients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
