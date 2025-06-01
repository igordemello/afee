using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class PartidaRecebeChaveamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChaveamentoId",
                table: "Partidas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_ChaveamentoId",
                table: "Partidas",
                column: "ChaveamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidas_Chaveamentos_ChaveamentoId",
                table: "Partidas",
                column: "ChaveamentoId",
                principalTable: "Chaveamentos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidas_Chaveamentos_ChaveamentoId",
                table: "Partidas");

            migrationBuilder.DropIndex(
                name: "IX_Partidas_ChaveamentoId",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "ChaveamentoId",
                table: "Partidas");
        }
    }
}
