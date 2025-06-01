using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClasseAndIdadeInPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Classe",
                table: "Players",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Classe",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Players");
        }
    }
}
