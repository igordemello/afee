using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTreinoDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Horario",
                table: "Treinos");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HorarioFim",
                table: "Treinos",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "Data",
                table: "Treinos",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioInicio",
                table: "Treinos",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Treinos");

            migrationBuilder.DropColumn(
                name: "HorarioInicio",
                table: "Treinos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HorarioFim",
                table: "Treinos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<DateTime>(
                name: "Horario",
                table: "Treinos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
