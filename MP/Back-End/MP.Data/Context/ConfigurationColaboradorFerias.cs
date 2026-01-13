using ControleFerias.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFerias.Infraestructure.Data
{
    public class ConfigurationColaboradorFerias : IEntityTypeConfiguration<ColaboradorFerias>
    {
        public void Configure(EntityTypeBuilder<ColaboradorFerias> builder)
        {
            builder.ToTable("ColaboradorFerias", "public");

            builder.HasKey(cf => new { cf.FeriasId, cf.ColaboradorId });

            builder.HasOne(cf => cf.Colaborador)
                   .WithMany(c => c.ColaboradorFerias)
                   .HasForeignKey(cf => cf.ColaboradorId);

            builder.HasOne(cf => cf.Ferias)
                   .WithMany(f => f.ColaboradorFerias)
                   .HasForeignKey(cf => cf.FeriasId);
        }
    }
}
