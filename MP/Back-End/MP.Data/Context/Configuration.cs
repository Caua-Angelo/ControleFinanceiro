using ControleFerias.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFerias.Infraestructure.Data
{
	public class Configuration : IEntityTypeConfiguration<Colaborador>
	{
		public void Configure(EntityTypeBuilder<Colaborador> builder)
		{
			builder.HasMany(x => x.Ferias)
					.WithMany(x => x.Colaborador)
					.UsingEntity<ColaboradorFerias>(
						x => x.HasOne(x => x.Ferias).WithMany(x => x.ColaboradorFerias).HasForeignKey(x => x.FeriasId),
						x => x.HasOne(x => x.Colaborador).WithMany(x => x.ColaboradorFerias).HasForeignKey(x => x.ColaboradorId),
						x =>
						{
							x.ToTable("ColaboradorFerias");

							x.HasKey(p => new { p.FeriasId, p.ColaboradorId });
						}
				);
		}
	}
}
