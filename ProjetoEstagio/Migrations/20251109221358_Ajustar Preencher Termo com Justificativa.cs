using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoEstagio.Migrations
{
    /// <inheritdoc />
    public partial class AjustarPreencherTermocomJustificativa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TermosCompromisso_SolicitacaoEstagioId",
                table: "TermosCompromisso");

            migrationBuilder.AddColumn<string>(
                name: "Justificativa",
                table: "TermosCompromisso",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TermosCompromisso_SolicitacaoEstagioId",
                table: "TermosCompromisso",
                column: "SolicitacaoEstagioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TermosCompromisso_SolicitacaoEstagioId",
                table: "TermosCompromisso");

            migrationBuilder.DropColumn(
                name: "Justificativa",
                table: "TermosCompromisso");

            migrationBuilder.CreateIndex(
                name: "IX_TermosCompromisso_SolicitacaoEstagioId",
                table: "TermosCompromisso",
                column: "SolicitacaoEstagioId");
        }
    }
}
