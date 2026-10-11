using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infra.Data
{
    public class ConfigurationCartao : IEntityTypeConfiguration<Cartao>
    {
        public void Configure(EntityTypeBuilder<Cartao> builder)
        {
            builder.ToTable("Cartao");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Imagem)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.DiaFechamento)
                   .IsRequired();

            builder.Property(c => c.DiaVencimento)
                   .IsRequired();

            // Relacionamento N -> 1 (Cartão pertence a um Usuário)
            builder.HasOne(c => c.Usuario)
                   .WithMany()
                   .HasForeignKey(c => c.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
