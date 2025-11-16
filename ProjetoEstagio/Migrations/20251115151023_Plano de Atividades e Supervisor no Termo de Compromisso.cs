using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoEstagio.Migrations
{
    /// <inheritdoc />
    public partial class PlanodeAtividadeseSupervisornoTermodeCompromisso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanoDeAtividades",
                table: "TermosCompromisso",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "TermosCompromisso",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TermosCompromisso_SupervisorId",
                table: "TermosCompromisso",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TermosCompromisso_Supervisores_SupervisorId",
                table: "TermosCompromisso",
                column: "SupervisorId",
                principalTable: "Supervisores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermosCompromisso_Supervisores_SupervisorId",
                table: "TermosCompromisso");

            migrationBuilder.DropIndex(
                name: "IX_TermosCompromisso_SupervisorId",
                table: "TermosCompromisso");

            migrationBuilder.DropColumn(
                name: "PlanoDeAtividades",
                table: "TermosCompromisso");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "TermosCompromisso");
        }
    }
}
