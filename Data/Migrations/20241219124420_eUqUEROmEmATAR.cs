using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class eUqUEROmEmATAR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Torneios_Fases_FaseId",
                table: "Torneios");

            migrationBuilder.DropIndex(
                name: "IX_Torneios_FaseId",
                table: "Torneios");

            migrationBuilder.DropColumn(
                name: "FaseId",
                table: "Torneios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseId",
                table: "Torneios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Torneios_FaseId",
                table: "Torneios",
                column: "FaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Torneios_Fases_FaseId",
                table: "Torneios",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
