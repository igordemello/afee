using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class TreinadorIdInPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TreinadorId",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_TreinadorId",
                table: "Players",
                column: "TreinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Treinadores_TreinadorId",
                table: "Players",
                column: "TreinadorId",
                principalTable: "Treinadores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Treinadores_TreinadorId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_TreinadorId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TreinadorId",
                table: "Players");
        }
    }
}
