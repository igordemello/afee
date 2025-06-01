using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tcc_in305b.Data.Migrations
{
    /// <inheritdoc />
    public partial class AttEquipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Grupos_GrupoId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "AnaliseGrupos");

            migrationBuilder.DropTable(
                name: "AvaliacaoDesempenhos");

            migrationBuilder.DropTable(
                name: "PartidaFases");

            migrationBuilder.DropTable(
                name: "RoundEquipe");

            migrationBuilder.DropTable(
                name: "SelecaoPlayers");

            migrationBuilder.DropTable(
                name: "TorneioFases");

            migrationBuilder.DropTable(
                name: "TreinadorFases");

            migrationBuilder.DropTable(
                name: "Partidas");

            migrationBuilder.DropTable(
                name: "Torneios");

            migrationBuilder.DropTable(
                name: "Fases");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Players_FaseId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Nota",
                table: "Equipes");

            migrationBuilder.AddColumn<int>(
                name: "EquipeId",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DtCriacao",
                table: "Equipes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Equipes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Equipes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Players_EquipeId",
                table: "Players",
                column: "EquipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Equipes_EquipeId",
                table: "Players",
                column: "EquipeId",
                principalTable: "Equipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Grupos_GrupoId",
                table: "Players",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Equipes_EquipeId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Grupos_GrupoId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_EquipeId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "EquipeId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DtCriacao",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Equipes");

            migrationBuilder.AddColumn<double>(
                name: "Nota",
                table: "Equipes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "AnaliseGrupos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrupoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    Formacao = table.Column<string>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaliseGrupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnaliseGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnaliseGrupos_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvaliacaoDesempenhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    NotaComunicacao = table.Column<double>(type: "REAL", nullable: false),
                    NotaControleMapa = table.Column<double>(type: "REAL", nullable: false),
                    NotaGeral = table.Column<double>(type: "REAL", nullable: false),
                    NotaMira = table.Column<double>(type: "REAL", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rounds_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelecaoPlayers",
                columns: table => new
                {
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelecaoPlayers", x => new { x.SelecaoId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_SelecaoPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelecaoPlayers_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Torneios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SelecaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Convidado = table.Column<string>(type: "TEXT", nullable: false),
                    Resultado = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torneios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Torneios_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreinadorFases",
                columns: table => new
                {
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TreinadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreinadorFases", x => new { x.FaseId, x.TreinadorId });
                    table.ForeignKey(
                        name: "FK_TreinadorFases_Fases_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreinadorFases_Treinadores_TreinadorId",
                        column: x => x.TreinadorId,
                        principalTable: "Treinadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoundId = table.Column<int>(type: "INTEGER", nullable: true),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidas_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoundEquipe",
                columns: table => new
                {
                    RoundId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundEquipe", x => new { x.RoundId, x.EquipeId });
                    table.ForeignKey(
                        name: "FK_RoundEquipe_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoundEquipe_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TorneioFases",
                columns: table => new
                {
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TorneioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorneioFases", x => new { x.FaseId, x.TorneioId });
                    table.ForeignKey(
                        name: "FK_TorneioFases_Fases_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TorneioFases_Torneios_TorneioId",
                        column: x => x.TorneioId,
                        principalTable: "Torneios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartidaFases",
                columns: table => new
                {
                    FaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartidaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidaFases", x => new { x.FaseId, x.PartidaId });
                    table.ForeignKey(
                        name: "FK_PartidaFases_Fases_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartidaFases_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_FaseId",
                table: "Players",
                column: "FaseId");

            migrationBuilder.CreateIndex(
                name: "IX_AnaliseGrupos_GrupoId",
                table: "AnaliseGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_AnaliseGrupos_TreinadorId",
                table: "AnaliseGrupos",
                column: "TreinadorId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PartidaFases_PartidaId",
                table: "PartidaFases",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_RoundId",
                table: "Partidas",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundEquipe_EquipeId",
                table: "RoundEquipe",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_SelecaoId",
                table: "Rounds",
                column: "SelecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_SelecaoPlayers_PlayerId",
                table: "SelecaoPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TorneioFases_TorneioId",
                table: "TorneioFases",
                column: "TorneioId");

            migrationBuilder.CreateIndex(
                name: "IX_Torneios_SelecaoId",
                table: "Torneios",
                column: "SelecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_TreinadorFases_TreinadorId",
                table: "TreinadorFases",
                column: "TreinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Fases_FaseId",
                table: "Players",
                column: "FaseId",
                principalTable: "Fases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Grupos_GrupoId",
                table: "Players",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id");
        }
    }
}
