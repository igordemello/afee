using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAvaliacaoDesempenho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvaliacaoDesempenhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NotaGeral = table.Column<double>(type: "REAL", nullable: false),
                    NotaMira = table.Column<double>(type: "REAL", nullable: false),
                    NotaControleMapa = table.Column<double>(type: "REAL", nullable: false),
                    NotaComunicacao = table.Column<double>(type: "REAL", nullable: false),
                    NotaNocao = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacaoDesempenhos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacaoDesempenhos_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvaliacaoDesempenhos_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvaliacaoDesempenhos_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacaoDesempenhos_PlayerId",
                table: "AvaliacaoDesempenhos",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacaoDesempenhos_SelecaoId",
                table: "AvaliacaoDesempenhos",
                column: "SelecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacaoDesempenhos_TreinadorId",
                table: "AvaliacaoDesempenhos",
                column: "TreinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaliacaoDesempenhos");
        }
    }
}
