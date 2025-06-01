using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class CollectionFasePlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseId1",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_FaseId1",
                table: "Players",
                column: "FaseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fases_FaseId1",
                table: "Players",
                column: "FaseId1",
                principalTable: "Fases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fases_FaseId1",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_FaseId1",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FaseId1",
                table: "Players");
        }
    }
}
