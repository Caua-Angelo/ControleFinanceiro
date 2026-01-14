using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infraestructure.Data
{
    public class ConfigurationCategoria : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categoria");

            // 🔹 Chave primária
            builder.HasKey(c => c.Id);

            // 🔹 Descrição
            builder.Property(c => c.Descricao)
                   .IsRequired()
                   .HasMaxLength(100);

            // 🔹 Finalidade (enum)
            builder.Property(c => c.Finalidade)
                   .IsRequired()
                   .HasConversion<int>();

            // 🔹 Relacionamento 1 -> N (Categoria tem várias Transações)
            builder.HasMany(c => c.Transacoes)
                   .WithOne(t => t.Categoria)
                   .HasForeignKey(t => t.CategoriaId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
