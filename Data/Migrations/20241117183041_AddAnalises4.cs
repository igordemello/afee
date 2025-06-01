using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalises4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnaliseNota_Analises_AnaliseId",
                table: "AnaliseNota");

            migrationBuilder.DropForeignKey(
                name: "FK_AnaliseNota_Notas_NotaId",
                table: "AnaliseNota");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnaliseNota",
                table: "AnaliseNota");

            migrationBuilder.RenameTable(
                name: "AnaliseNota",
                newName: "AnaliseNotas");

            migrationBuilder.RenameIndex(
                name: "IX_AnaliseNota_NotaId",
                table: "AnaliseNotas",
                newName: "IX_AnaliseNotas_NotaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnaliseNotas",
                table: "AnaliseNotas",
                columns: new[] { "AnaliseId", "NotaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnaliseNotas_Analises_AnaliseId",
                table: "AnaliseNotas",
                column: "AnaliseId",
                principalTable: "Analises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnaliseNotas_Notas_NotaId",
                table: "AnaliseNotas",
                column: "NotaId",
                principalTable: "Notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnaliseNotas_Analises_AnaliseId",
                table: "AnaliseNotas");

            migrationBuilder.DropForeignKey(
                name: "FK_AnaliseNotas_Notas_NotaId",
                table: "AnaliseNotas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnaliseNotas",
                table: "AnaliseNotas");

            migrationBuilder.RenameTable(
                name: "AnaliseNotas",
                newName: "AnaliseNota");

            migrationBuilder.RenameIndex(
                name: "IX_AnaliseNotas_NotaId",
                table: "AnaliseNota",
                newName: "IX_AnaliseNota_NotaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnaliseNota",
                table: "AnaliseNota",
                columns: new[] { "AnaliseId", "NotaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnaliseNota_Analises_AnaliseId",
                table: "AnaliseNota",
                column: "AnaliseId",
                principalTable: "Analises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnaliseNota_Notas_NotaId",
                table: "AnaliseNota",
                column: "NotaId",
                principalTable: "Notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
