using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infraestructure.Data
{
    public class ConfigurationUsuario : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");

            // Chave primária
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Idade)
                   .IsRequired();

            // 🔹 Relacionamento 1 -> N (Usuário tem várias Transações)
            builder.HasMany(u => u.Transacao)
                   .WithOne(t => t.Usuario)
                   .HasForeignKey(t => t.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
