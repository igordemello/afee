using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AttTreinoTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreinoTipos_Treinos_TreinoId",
                table: "TreinoTipos");

            migrationBuilder.DropIndex(
                name: "IX_TreinoTipos_TreinoId",
                table: "TreinoTipos");

            migrationBuilder.DropColumn(
                name: "TreinoId",
                table: "TreinoTipos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TreinoId",
                table: "TreinoTipos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreinoTipos_TreinoId",
                table: "TreinoTipos",
                column: "TreinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreinoTipos_Treinos_TreinoId",
                table: "TreinoTipos",
                column: "TreinoId",
                principalTable: "Treinos",
                principalColumn: "Id");
        }
    }
}
