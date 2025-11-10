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
        public DbSet<OrientadorModel> Orientadores { get; set; }
        public DbSet<SolicitacaoEstagioModel> SolicitacoesEstagio { get; set; }
        public DbSet<TermoCompromissoModel> TermosCompromisso { get; set; }

        // Em: Data/ProjetoEstagioContext.cs

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- INÍCIO DA CONFIGURAÇÃO DE ÍNDICES ÚNICOS ---
            // (SEU CÓDIGO EXISTENTE - ESTÁ CORRETO)

            // Garante que o CPF é único na tabela de Estagiários
            modelBuilder.Entity<EstagiarioModel>(entity =>
            {
                entity.HasIndex(e => e.CPF)
                      .IsUnique();
            });

            // Garante que o Email é único na tabela de Usuários
            modelBuilder.Entity<UsuarioModel>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                // Você provavelmente também quer que o Login seja único
                entity.HasIndex(u => u.Login)
                      .IsUnique();
            });

            // --- FIM DA CONFIGURAÇÃO DE ÍNDICES ---


            // --- INÍCIO DAS NOVAS REGRAS 'ON DELETE' ---
            // (ADICIONE ESTE BLOCO)

            // Regra para Empresa -> Usuario
            modelBuilder.Entity<EmpresaModel>()
                .HasOne(e => e.Usuario)
                .WithMany() // Ou WithOne(), se Usuario tiver EmpresaModel
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction); // <-- A CORREÇÃO

            // Regra para Estagiario -> Usuario
            modelBuilder.Entity<EstagiarioModel>()
                .HasOne(e => e.Usuario)
                .WithMany() // Ou WithOne()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction); // <-- A CORREÇÃO

            // Regra para Supervisor -> Usuario (A que causou o erro)
            modelBuilder.Entity<SupervisorModel>()
                .HasOne(s => s.Usuario)
                .WithMany() // Ou WithOne()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction); // <-- A CORREÇÃO

            // Regra para Supervisor -> Empresa (Esta pode ficar em cascata)
            modelBuilder.Entity<SupervisorModel>()
                .HasOne(s => s.Empresa)
                .WithMany(e => e.Supervisores) // Assumindo que Empresa tem ICollection<SupervisorModel>
                .HasForeignKey(s => s.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade); // Deletar a empresa deleta o supervisor

            // --- FIM DAS NOVAS REGRAS 'ON DELETE' ---
        }
    }
}
