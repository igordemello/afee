using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class addfaseTreinador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fases",
                table: "Fases");

            migrationBuilder.RenameTable(
                name: "Fases",
                newName: "Fase");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fase",
                table: "Fase",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FaseTreinador",
                columns: table => new
                {
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaseTreinador", x => new { x.FaseId, x.TreinadorId });
                    table.ForeignKey(
                        name: "FK_FaseTreinador_Fase_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaseTreinador_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaseTreinador_TreinadorId",
                table: "FaseTreinador",
                column: "TreinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fase_FaseId",
                table: "Players",
                column: "FaseId",
                principalTable: "Fase",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fase_FaseId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "FaseTreinador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fase",
                table: "Fase");

            migrationBuilder.RenameTable(
                name: "Fase",
                newName: "Fases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fases",
                table: "Fases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id");
        }
    }
}
