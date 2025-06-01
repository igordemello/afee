using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class TreinadorNoTorneio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TreinadorId",
                table: "Torneios",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Torneios_TreinadorId",
                table: "Torneios",
                column: "TreinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Torneios_Treinadores_TreinadorId",
                table: "Torneios",
                column: "TreinadorId",
                principalTable: "Treinadores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Torneios_Treinadores_TreinadorId",
                table: "Torneios");

            migrationBuilder.DropIndex(
                name: "IX_Torneios_TreinadorId",
                table: "Torneios");

            migrationBuilder.DropColumn(
                name: "TreinadorId",
                table: "Torneios");
        }
    }
}
