
using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infra.Data
{
    public class ConfigurationParcela
        : IEntityTypeConfiguration<Parcela>
    {
        public void Configure(EntityTypeBuilder<Parcela> builder)
        {
            builder.ToTable("Parcela");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Numero)
                .IsRequired();

            builder.Property(p => p.Valor)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.DataVencimento)
                .IsRequired();

            builder.Property(p => p.Paga)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(p => p.CompraParcelada)
                .WithMany(c => c.Parcelas)
                .HasForeignKey(p => p.CompraParceladaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Impede duas parcelas com o mesmo número
            // dentro da mesma compra.
            builder.HasIndex(p => new
            {
                p.CompraParceladaId,
                p.Numero
            })
                .IsUnique();
        }
    }
}
