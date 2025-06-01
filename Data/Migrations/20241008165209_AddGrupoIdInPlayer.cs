using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGrupoIdInPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrupoId",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_GrupoId",
                table: "Players",
                column: "GrupoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Grupos_GrupoId",
                table: "Players",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Grupos_GrupoId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_GrupoId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GrupoId",
                table: "Players");
        }
    }
}
