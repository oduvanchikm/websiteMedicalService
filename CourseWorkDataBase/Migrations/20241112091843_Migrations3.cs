using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class Migrations3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Password", "PersonalNumber", "RoleId" },
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin@example.com", "$2a$11$o.sTnyjh8Mr9ArOWpr5Q..rsRPFHJ7EJ6pIeFUyVEfP2fe5b1riHm", null, 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
