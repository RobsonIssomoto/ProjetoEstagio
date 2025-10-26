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
    }
}
