using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoEstagio.Migrations
{
    /// <inheritdoc />
    public partial class OrientadorEstágioeTermodeCompromisso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orientadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orientadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orientadores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitacoesEstagio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataSubmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstagiarioId = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesEstagio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesEstagio_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitacoesEstagio_Estagiarios_EstagiarioId",
                        column: x => x.EstagiarioId,
                        principalTable: "Estagiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermosCompromisso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargaHoraria = table.Column<int>(type: "int", nullable: false),
                    ValorBolsa = table.Column<double>(type: "float", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumeroApolice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeSeguradora = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolicitacaoEstagioId = table.Column<int>(type: "int", nullable: false),
                    OrientadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermosCompromisso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermosCompromisso_Orientadores_OrientadorId",
                        column: x => x.OrientadorId,
                        principalTable: "Orientadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermosCompromisso_SolicitacoesEstagio_SolicitacaoEstagioId",
                        column: x => x.SolicitacaoEstagioId,
                        principalTable: "SolicitacoesEstagio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orientadores_UsuarioId",
                table: "Orientadores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEstagio_EmpresaId",
                table: "SolicitacoesEstagio",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEstagio_EstagiarioId",
                table: "SolicitacoesEstagio",
                column: "EstagiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TermosCompromisso_OrientadorId",
                table: "TermosCompromisso",
                column: "OrientadorId");

            migrationBuilder.CreateIndex(
                name: "IX_TermosCompromisso_SolicitacaoEstagioId",
                table: "TermosCompromisso",
                column: "SolicitacaoEstagioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermosCompromisso");

            migrationBuilder.DropTable(
                name: "Orientadores");

            migrationBuilder.DropTable(
                name: "SolicitacoesEstagio");
        }
    }
}
