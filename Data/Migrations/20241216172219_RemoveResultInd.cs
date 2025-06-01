using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResultInd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultIndSelecaoNotas");

            migrationBuilder.DropTable(
                name: "ResultIndSelecoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultIndSelecoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultIndSelecoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultIndSelecoes_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultIndSelecoes_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultIndSelecaoNotas",
                columns: table => new
                {
                    ResultIndSelecaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NotaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultIndSelecaoNotas", x => new { x.ResultIndSelecaoId, x.NotaId });
                    table.ForeignKey(
                        name: "FK_ResultIndSelecaoNotas_Notas_NotaId",
                        column: x => x.NotaId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultIndSelecaoNotas_ResultIndSelecoes_ResultIndSelecaoId",
                        column: x => x.ResultIndSelecaoId,
                        principalTable: "ResultIndSelecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultIndSelecaoNotas_NotaId",
                table: "ResultIndSelecaoNotas",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultIndSelecoes_PlayerId",
                table: "ResultIndSelecoes",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultIndSelecoes_SelecaoId",
                table: "ResultIndSelecoes",
                column: "SelecaoId");
        }
    }
}
