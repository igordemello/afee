using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTreinoTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Treinos");

            migrationBuilder.AddColumn<int>(
                name: "TreinoTipoId",
                table: "Treinos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TreinoTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreinoTipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreinoTipos_Treinos_TreinoId",
                        column: x => x.TreinoId,
                        principalTable: "Treinos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TreinoTipos_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Treinos_TreinoTipoId",
                table: "Treinos",
                column: "TreinoTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_TreinoTipos_TreinoId",
                table: "TreinoTipos",
                column: "TreinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TreinoTipos_TreinadorId",
                table: "TreinoTipos",
                column: "TreinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Treinos_TreinoTipos_TreinoTipoId",
                table: "Treinos",
                column: "TreinoTipoId",
                principalTable: "TreinoTipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Treinos_TreinoTipos_TreinoTipoId",
                table: "Treinos");

            migrationBuilder.DropTable(
                name: "TreinoTipos");

            migrationBuilder.DropIndex(
                name: "IX_Treinos_TreinoTipoId",
                table: "Treinos");

            migrationBuilder.DropColumn(
                name: "TreinoTipoId",
                table: "Treinos");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Treinos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
