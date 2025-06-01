using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class SelecaoIdinEquipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Equipes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Equipes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "SelecaoId",
                table: "Equipes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_SelecaoId",
                table: "Equipes",
                column: "SelecaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipes_Selecoes_SelecaoId",
                table: "Equipes",
                column: "SelecaoId",
                principalTable: "Selecoes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Selecoes_SelecaoId",
                table: "Equipes");

            migrationBuilder.DropIndex(
                name: "IX_Equipes_SelecaoId",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "SelecaoId",
                table: "Equipes");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Equipes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Equipes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
