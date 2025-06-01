using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixFasesFaseTreinadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinador_Fase_FaseId",
                table: "FaseTreinador");

            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinador_Treinadores_TreinadorId",
                table: "FaseTreinador");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fase_FaseId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaseTreinador",
                table: "FaseTreinador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fase",
                table: "Fase");

            migrationBuilder.RenameTable(
                name: "FaseTreinador",
                newName: "FaseTreinadores");

            migrationBuilder.RenameTable(
                name: "Fase",
                newName: "Fases");

            migrationBuilder.RenameIndex(
                name: "IX_FaseTreinador_TreinadorId",
                table: "FaseTreinadores",
                newName: "IX_FaseTreinadores_TreinadorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaseTreinadores",
                table: "FaseTreinadores",
                columns: new[] { "FaseId", "TreinadorId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fases",
                table: "Fases",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinadores_Fases_FaseId",
                table: "FaseTreinadores");

            migrationBuilder.DropForeignKey(
                name: "FK_FaseTreinadores_Treinadores_TreinadorId",
                table: "FaseTreinadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaseTreinadores",
                table: "FaseTreinadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fases",
                table: "Fases");

            migrationBuilder.RenameTable(
                name: "FaseTreinadores",
                newName: "FaseTreinador");

            migrationBuilder.RenameTable(
                name: "Fases",
                newName: "Fase");

            migrationBuilder.RenameIndex(
                name: "IX_FaseTreinadores_TreinadorId",
                table: "FaseTreinador",
                newName: "IX_FaseTreinador_TreinadorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaseTreinador",
                table: "FaseTreinador",
                columns: new[] { "FaseId", "TreinadorId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fase",
                table: "Fase",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaseTreinador_Fase_FaseId",
                table: "FaseTreinador",
                column: "FaseId",
                principalTable: "Fase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaseTreinador_Treinadores_TreinadorId",
                table: "FaseTreinador",
                column: "TreinadorId",
                principalTable: "Treinadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fase_FaseId",
                table: "Players",
                column: "FaseId",
                principalTable: "Fase",
                principalColumn: "Id");
        }
    }
}
