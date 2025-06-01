using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTreinadorFaseRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaseTreinadores",
                columns: table => new
                {
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaseTreinadores", x => new { x.FaseId, x.TreinadorId });
                    table.ForeignKey(
                        name: "FK_FaseTreinadores_Fases_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaseTreinadores_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaseTreinadores_TreinadorId",
                table: "FaseTreinadores",
                column: "TreinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaseTreinadores");
        }
    }
}
