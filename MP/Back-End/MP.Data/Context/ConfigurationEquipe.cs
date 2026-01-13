using ControleFerias.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFerias.Infraestructure.Data
{
    public class ConfigurationEquipe : IEntityTypeConfiguration<Equipe>
    {
        public void Configure(EntityTypeBuilder<Equipe> builder)
        {
            // Define o nome da tabela
            builder.ToTable("Equipe", "public");


            builder.HasKey(e => e.Id);


            builder.Property(e => e.sNome)
                   .IsRequired()
                   .HasColumnType("text")
                   .HasMaxLength(255);

            // Relacionamento com Colaborador (1:N)
            builder.HasMany(e => e.Colaboradores)
                       .WithOne(c => c.Equipe)
                       .HasForeignKey(c => c.EquipeId)
                       .OnDelete(DeleteBehavior.Cascade); // define o comportamento de delete

        }
    }
}
