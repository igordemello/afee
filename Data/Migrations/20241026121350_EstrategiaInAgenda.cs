using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstrategiaInTreino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstrategiaId",
                table: "Treinos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Treinos_EstrategiaId",
                table: "Treinos",
                column: "EstrategiaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Treinos_Estrategias_EstrategiaId",
                table: "Treinos",
                column: "EstrategiaId",
                principalTable: "Estrategias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Treinos_Estrategias_EstrategiaId",
                table: "Treinos");

            migrationBuilder.DropIndex(
                name: "IX_Treinos_EstrategiaId",
                table: "Treinos");

            migrationBuilder.DropColumn(
                name: "EstrategiaId",
                table: "Treinos");
        }
    }
}
