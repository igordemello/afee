using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalises2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalisePlayerGrupo_Analises_AnaliseId",
                table: "AnalisePlayerGrupo");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalisePlayerGrupo_Grupos_GrupoId",
                table: "AnalisePlayerGrupo");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalisePlayerGrupo_Players_PlayerId",
                table: "AnalisePlayerGrupo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalisePlayerGrupo",
                table: "AnalisePlayerGrupo");

            migrationBuilder.RenameTable(
                name: "AnalisePlayerGrupo",
                newName: "AnalisePlayerGrupos");

            migrationBuilder.RenameIndex(
                name: "IX_AnalisePlayerGrupo_PlayerId",
                table: "AnalisePlayerGrupos",
                newName: "IX_AnalisePlayerGrupos_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalisePlayerGrupo_GrupoId",
                table: "AnalisePlayerGrupos",
                newName: "IX_AnalisePlayerGrupos_GrupoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalisePlayerGrupos",
                table: "AnalisePlayerGrupos",
                columns: new[] { "AnaliseId", "PlayerId", "GrupoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnalisePlayerGrupos_Analises_AnaliseId",
                table: "AnalisePlayerGrupos",
                column: "AnaliseId",
                principalTable: "Analises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalisePlayerGrupos_Grupos_GrupoId",
                table: "AnalisePlayerGrupos",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalisePlayerGrupos_Players_PlayerId",
                table: "AnalisePlayerGrupos",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalisePlayerGrupos_Analises_AnaliseId",
                table: "AnalisePlayerGrupos");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalisePlayerGrupos_Grupos_GrupoId",
                table: "AnalisePlayerGrupos");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalisePlayerGrupos_Players_PlayerId",
                table: "AnalisePlayerGrupos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalisePlayerGrupos",
                table: "AnalisePlayerGrupos");

            migrationBuilder.RenameTable(
                name: "AnalisePlayerGrupos",
                newName: "AnalisePlayerGrupo");

            migrationBuilder.RenameIndex(
                name: "IX_AnalisePlayerGrupos_PlayerId",
                table: "AnalisePlayerGrupo",
                newName: "IX_AnalisePlayerGrupo_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalisePlayerGrupos_GrupoId",
                table: "AnalisePlayerGrupo",
                newName: "IX_AnalisePlayerGrupo_GrupoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalisePlayerGrupo",
                table: "AnalisePlayerGrupo",
                columns: new[] { "AnaliseId", "PlayerId", "GrupoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnalisePlayerGrupo_Analises_AnaliseId",
                table: "AnalisePlayerGrupo",
                column: "AnaliseId",
                principalTable: "Analises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalisePlayerGrupo_Grupos_GrupoId",
                table: "AnalisePlayerGrupo",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalisePlayerGrupo_Players_PlayerId",
                table: "AnalisePlayerGrupo",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
