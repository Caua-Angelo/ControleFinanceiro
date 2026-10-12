
using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infra.Data
{
    public class ConfigurationCompraParcelada
        : IEntityTypeConfiguration<CompraParcelada>
    {
        public void Configure(EntityTypeBuilder<CompraParcelada> builder)
        {
            builder.ToTable("CompraParcelada");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Descricao)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ValorTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.QuantidadeParcelas)
                .IsRequired();

            builder.Property(c => c.DataCompra)
                .IsRequired();

            builder.HasOne(c => c.Cartao)
                .WithMany()
                .HasForeignKey(c => c.CartaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Parcelas)
                .WithOne(p => p.CompraParcelada)
                .HasForeignKey(p => p.CompraParceladaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.UsuarioId);
            builder.HasIndex(c => c.CartaoId);
        }
    }
}
