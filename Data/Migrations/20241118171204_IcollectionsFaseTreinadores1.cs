using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class IcollectionsFaseTreinadores1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinadores_Fases_FaseId1",
                table: "FaseTreinadores");

            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinadores_Treinadores_TreinadorId1",
                table: "FaseTreinadores");

            migrationBuilder.DropIndex(
                name: "IX_FaseTreinadores_FaseId1",
                table: "FaseTreinadores");

            migrationBuilder.DropIndex(
                name: "IX_FaseTreinadores_TreinadorId1",
                table: "FaseTreinadores");

            migrationBuilder.DropColumn(
                name: "FaseId1",
                table: "FaseTreinadores");

            migrationBuilder.DropColumn(
                name: "TreinadorId1",
                table: "FaseTreinadores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseId1",
                table: "FaseTreinadores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TreinadorId1",
                table: "FaseTreinadores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaseTreinadores_FaseId1",
                table: "FaseTreinadores",
                column: "FaseId1");

            migrationBuilder.CreateIndex(
                name: "IX_FaseTreinadores_TreinadorId1",
                table: "FaseTreinadores",
                column: "TreinadorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FaseTreinadores_Fases_FaseId1",
                table: "FaseTreinadores",
                column: "FaseId1",
                principalTable: "Fases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaseTreinadores_Treinadores_TreinadorId1",
                table: "FaseTreinadores",
                column: "TreinadorId1",
                principalTable: "Treinadores",
                principalColumn: "Id");
        }
    }
}
