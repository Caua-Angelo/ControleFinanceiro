using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infraestructure.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Transacao> Transacao{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.HasDefaultSchema("public");

            builder.ApplyConfiguration(new ConfigurationUsuario());
            builder.ApplyConfiguration(new ConfigurationCategoria());
            builder.ApplyConfiguration(new ConfigurationTransacao());

            base.OnModelCreating(builder);
        }
    }
}
