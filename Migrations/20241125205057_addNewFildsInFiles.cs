using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlobStorage.Migrations
{
    /// <inheritdoc />
    public partial class addNewFildsInFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Files");

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Files",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Version",
                table: "Files",
                type: "tinyint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Files");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Files",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
