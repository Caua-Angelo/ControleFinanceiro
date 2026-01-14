using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infraestructure.Data
{
    public class ConfigurationTransacao : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.ToTable("Transacao");

            // 🔹 Chave primária
            builder.HasKey(t => t.Id);

            // 🔹 Descrição
            builder.Property(t => t.Descricao)
                   .IsRequired()
                   .HasMaxLength(100);

            // 🔹 Valor
            builder.Property(t => t.Valor)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            // 🔹 Tipo (enum)
            builder.Property(t => t.Tipo)
                   .IsRequired()
                   .HasConversion<int>();

            // 🔹 Relacionamento com Categoria (N -> 1)
            builder.HasOne(t => t.Categoria)
                   .WithMany(c => c.Transacoes)
                   .HasForeignKey(t => t.CategoriaId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Relacionamento com Usuario (N -> 1)
            builder.HasOne(t => t.Usuario)
                   .WithMany(u => u.Transacao)
                   .HasForeignKey(t => t.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
