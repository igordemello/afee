using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEstrategia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estrategias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descrição = table.Column<string>(type: "TEXT", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estrategias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estrategias_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstrategiaGrupos",
                columns: table => new
                {
                    GrupoId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstrategiaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstrategiaGrupos", x => new { x.EstrategiaId, x.GrupoId });
                    table.ForeignKey(
                        name: "FK_EstrategiaGrupos_Estrategias_EstrategiaId",
                        column: x => x.EstrategiaId,
                        principalTable: "Estrategias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstrategiaGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstrategiaGrupos_GrupoId",
                table: "EstrategiaGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estrategias_TreinadorId",
                table: "Estrategias",
                column: "TreinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstrategiaGrupos");

            migrationBuilder.DropTable(
                name: "Estrategias");
        }
    }
}
