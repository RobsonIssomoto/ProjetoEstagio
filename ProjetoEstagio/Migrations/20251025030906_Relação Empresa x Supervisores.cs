using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoEstagio.Migrations
{
    /// <inheritdoc />
    public partial class RelaçãoEmpresaxSupervisores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Supervisores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Supervisores_EmpresaId",
                table: "Supervisores",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisores_Empresas_EmpresaId",
                table: "Supervisores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supervisores_Empresas_EmpresaId",
                table: "Supervisores");

            migrationBuilder.DropIndex(
                name: "IX_Supervisores_EmpresaId",
                table: "Supervisores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Supervisores");
        }
    }
}
