using ControleFerias.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFerias.Infraestructure.Data
{
    public class ConfigurationFerias : IEntityTypeConfiguration<Ferias>
    {
        public void Configure(EntityTypeBuilder<Ferias> builder)
        {
            // Nome da tabela e schema
            builder.ToTable("Ferias", "public");

            // Chave primária
            builder.HasKey(f => f.Id);

            // Coluna de data de início
            builder.Property(f => f.dDataInicio)
                   .IsRequired()
                   .HasColumnType("timestamp"); // PostgreSQL usa timestamp para DateTime

            // Coluna de dias
            builder.Property(f => f.sDias)
                   .IsRequired()
                   .HasColumnType("integer"); // tipo inteiro no PostgreSQL

            // Coluna de data final
            builder.Property(f => f.dDataFinal)
                   .IsRequired()
                   .HasColumnType("timestamp");

            // Coluna de status (enum convertido para texto)
            builder.Property(f => f.Status)
                  .IsRequired()
                  .HasConversion<string>()   // converte o enum em string
                  .HasMaxLength(20)          // previne erros de tamanho no PostgreSQL
                  .HasColumnType("varchar(20)");

            // Coluna de comentário (opcional)
            builder.Property(f => f.sComentario)
                   .HasColumnType("text")
                   .HasMaxLength(200)
                   .IsRequired(false);

            // Relacionamento muitos-para-muitos com Colaborador via ColaboradorFerias
            builder.HasMany(f => f.ColaboradorFerias)
                 .WithOne(cf => cf.Ferias)
                 .HasForeignKey(cf => cf.FeriasId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}





