using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTreinadorFaseRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinadores_Fases_FaseId",
                table: "FaseTreinadores");

            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinadores_Treinadores_TreinadorId",
                table: "FaseTreinadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaseTreinadores",
                table: "FaseTreinadores");

            migrationBuilder.RenameTable(
                name: "FaseTreinadores",
                newName: "TreinadorFases");

            migrationBuilder.RenameIndex(
                name: "IX_FaseTreinadores_TreinadorId",
                table: "TreinadorFases",
                newName: "IX_TreinadorFases_TreinadorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TreinadorFases",
                table: "TreinadorFases",
                columns: new[] { "FaseId", "TreinadorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TreinadorFases_Fases_FaseId",
                table: "TreinadorFases",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreinadorFases_Treinadores_TreinadorId",
                table: "TreinadorFases",
                column: "TreinadorId",
                principalTable: "Treinadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreinadorFases_Fases_FaseId",
                table: "TreinadorFases");

            migrationBuilder.DropForeignKey(
                name: "FK_TreinadorFases_Treinadores_TreinadorId",
                table: "TreinadorFases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TreinadorFases",
                table: "TreinadorFases");

            migrationBuilder.RenameTable(
                name: "TreinadorFases",
                newName: "FaseTreinadores");

            migrationBuilder.RenameIndex(
                name: "IX_TreinadorFases_TreinadorId",
                table: "FaseTreinadores",
                newName: "IX_FaseTreinadores_TreinadorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaseTreinadores",
                table: "FaseTreinadores",
                columns: new[] { "FaseId", "TreinadorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FaseTreinadores_Fases_FaseId",
                table: "FaseTreinadores",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaseTreinadores_Treinadores_TreinadorId",
                table: "FaseTreinadores",
                column: "TreinadorId",
                principalTable: "Treinadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
