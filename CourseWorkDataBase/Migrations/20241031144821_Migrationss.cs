﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class Migrationss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");
        }
    }
}
