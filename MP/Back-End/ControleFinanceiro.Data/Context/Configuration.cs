using ControleFinanceiro.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infraestructure.Data
{
	public class Configuration : IEntityTypeConfiguration<Usuario>
	{
		public void Configure(EntityTypeBuilder<Usuario> builder)
		{
			builder.HasMany(x => x.Ferias)
					.WithMany(x => x.Usuario)
					.UsingEntity<ColaboradorFerias>(
						x => x.HasOne(x => x.Ferias).WithMany(x => x.ColaboradorFerias).HasForeignKey(x => x.FeriasId),
						x => x.HasOne(x => x.Usuario).WithMany(x => x.ColaboradorFerias).HasForeignKey(x => x.ColaboradorId),
						x =>
						{
							x.ToTable("ColaboradorFerias");

							x.HasKey(p => new { p.FeriasId, p.ColaboradorId });
						}
				);
		}
	}
}
