using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseId",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Fases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_FaseId",
                table: "Players",
                column: "FaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Fases");

            migrationBuilder.DropIndex(
                name: "IX_Players_FaseId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FaseId",
                table: "Players");
        }
    }
}
