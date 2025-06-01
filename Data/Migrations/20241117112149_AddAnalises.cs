using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analises_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notas_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnaliseGrupos",
                columns: table => new
                {
                    AnaliseId = table.Column<int>(type: "INTEGER", nullable: false),
                    GrupoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Formacao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaliseGrupos", x => new { x.AnaliseId, x.GrupoId });
                    table.ForeignKey(
                        name: "FK_AnaliseGrupos_Analises_AnaliseId",
                        column: x => x.AnaliseId,
                        principalTable: "Analises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnaliseGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalisePlayerGrupo",
                columns: table => new
                {
                    AnaliseId = table.Column<int>(type: "INTEGER", nullable: false),
                    GrupoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisePlayerGrupo", x => new { x.AnaliseId, x.PlayerId, x.GrupoId });
                    table.ForeignKey(
                        name: "FK_AnalisePlayerGrupo_Analises_AnaliseId",
                        column: x => x.AnaliseId,
                        principalTable: "Analises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalisePlayerGrupo_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalisePlayerGrupo_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalisePlayers",
                columns: table => new
                {
                    AnaliseId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisePlayers", x => new { x.AnaliseId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_AnalisePlayers_Analises_AnaliseId",
                        column: x => x.AnaliseId,
                        principalTable: "Analises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalisePlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnaliseNota",
                columns: table => new
                {
                    AnaliseId = table.Column<int>(type: "INTEGER", nullable: false),
                    NotaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaliseNota", x => new { x.AnaliseId, x.NotaId });
                    table.ForeignKey(
                        name: "FK_AnaliseNota_Analises_AnaliseId",
                        column: x => x.AnaliseId,
                        principalTable: "Analises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnaliseNota_Notas_NotaId",
                        column: x => x.NotaId,
                        principalTable: "Notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnaliseGrupos_GrupoId",
                table: "AnaliseGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_AnaliseNota_NotaId",
                table: "AnaliseNota",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisePlayerGrupo_GrupoId",
                table: "AnalisePlayerGrupo",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisePlayerGrupo_PlayerId",
                table: "AnalisePlayerGrupo",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisePlayers_PlayerId",
                table: "AnalisePlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Analises_TreinadorId",
                table: "Analises",
                column: "TreinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notas_TreinadorId",
                table: "Notas",
                column: "TreinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnaliseGrupos");

            migrationBuilder.DropTable(
                name: "AnaliseNota");

            migrationBuilder.DropTable(
                name: "AnalisePlayerGrupo");

            migrationBuilder.DropTable(
                name: "AnalisePlayers");

            migrationBuilder.DropTable(
                name: "Notas");

            migrationBuilder.DropTable(
                name: "Analises");
        }
    }
}
