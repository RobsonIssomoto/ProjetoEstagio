using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Data
{
    public class ProjetoEstagioContext : DbContext
    {
        public ProjetoEstagioContext(DbContextOptions<ProjetoEstagioContext> options) : base(options) { }

        public DbSet<EmpresaModel> Empresas { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<SupervisorModel> Supervisores { get; set; }
        public DbSet<EstagiarioModel> Estagiarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- INÍCIO DA CONFIGURAÇÃO DE ÍNDICES ÚNICOS ---

            // Garante que o CPF é único na tabela de Estagiários
            modelBuilder.Entity<EstagiarioModel>(entity =>
            {
                entity.HasIndex(e => e.CPF)
                      .IsUnique();
            });

            // Garante que o Email é único na tabela de Usuários
            // (Pelo seu modelo, o Email principal de login está em UsuarioModel)
            modelBuilder.Entity<UsuarioModel>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                // Você provavelmente também quer que o Login seja único
                entity.HasIndex(u => u.Login)
                      .IsUnique();
            });

            // --- FIM DA CONFIGURAÇÃO ---
        }
    }
}
