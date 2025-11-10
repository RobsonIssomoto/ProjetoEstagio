using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoEstagio.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarioIdAoSupervisorcomregrasdeexclusãonoProjetoEstagioContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_Usuarios_UsuarioId",
                table: "Empresas");

            migrationBuilder.DropForeignKey(
                name: "FK_Estagiarios_Usuarios_UsuarioId",
                table: "Estagiarios");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Supervisores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Supervisores_UsuarioId",
                table: "Supervisores",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_Usuarios_UsuarioId",
                table: "Empresas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estagiarios_Usuarios_UsuarioId",
                table: "Estagiarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisores_Usuarios_UsuarioId",
                table: "Supervisores",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_Usuarios_UsuarioId",
                table: "Empresas");

            migrationBuilder.DropForeignKey(
                name: "FK_Estagiarios_Usuarios_UsuarioId",
                table: "Estagiarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Supervisores_Usuarios_UsuarioId",
                table: "Supervisores");

            migrationBuilder.DropIndex(
                name: "IX_Supervisores_UsuarioId",
                table: "Supervisores");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Supervisores");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_Usuarios_UsuarioId",
                table: "Empresas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Estagiarios_Usuarios_UsuarioId",
                table: "Estagiarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
