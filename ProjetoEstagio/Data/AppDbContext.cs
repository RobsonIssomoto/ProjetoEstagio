using Microsoft.EntityFrameworkCore;
using ProjetoEstagio.Models;

namespace ProjetoEstagio.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
        }
        public DbSet<Empresa> empresas {  get; set; }
    }
}
