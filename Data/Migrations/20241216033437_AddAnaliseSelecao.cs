using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnaliseSelecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnaliseSelecoes",
                columns: table => new
                {
                    AnaliseId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaliseSelecoes", x => new { x.AnaliseId, x.SelecaoId });
                    table.ForeignKey(
                        name: "FK_AnaliseSelecoes_Analises_AnaliseId",
                        column: x => x.AnaliseId,
                        principalTable: "Analises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnaliseSelecoes_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnaliseSelecoes_SelecaoId",
                table: "AnaliseSelecoes",
                column: "SelecaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnaliseSelecoes");
        }
    }
}
