using System;
using ControleFerias.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ControleFerias.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20250822182255_baseline")]
    partial class baseline
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ControleFerias.Models.Colaborador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EquipeId")
                        .HasColumnType("int");

                    b.Property<string>("sNome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EquipeId");

                    b.ToTable("Colaborador");
                });

            modelBuilder.Entity("ControleFerias.Models.ColaboradorFerias", b =>
                {
                    b.Property<int>("FeriasId")
                        .HasColumnType("int");

                    b.Property<int>("ColaboradorId")
                        .HasColumnType("int");

                    b.HasKey("FeriasId", "ColaboradorId");

                    b.HasIndex("ColaboradorId");

                    b.ToTable("ColaboradorFerias", (string)null);
                });

            modelBuilder.Entity("ControleFerias.Models.Equipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("sNome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Equipe");
                });

            modelBuilder.Entity("ControleFerias.Models.Ferias", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("dDataFinal")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("dDataInicio")
                        .HasColumnType("datetime2");

                    b.Property<string>("sComentario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sDias")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Ferias");
                });

            modelBuilder.Entity("ControleFerias.Models.Colaborador", b =>
                {
                    b.HasOne("ControleFerias.Models.Equipe", "Equipe")
                        .WithMany("Colaboradores")
                        .HasForeignKey("EquipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equipe");
                });

            modelBuilder.Entity("ControleFerias.Models.ColaboradorFerias", b =>
                {
                    b.HasOne("ControleFerias.Models.Colaborador", "Colaborador")
                        .WithMany("ColaboradorFerias")
                        .HasForeignKey("ColaboradorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ControleFerias.Models.Ferias", "Ferias")
                        .WithMany("ColaboradorFerias")
                        .HasForeignKey("FeriasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Colaborador");

                    b.Navigation("Ferias");
                });

            modelBuilder.Entity("ControleFerias.Models.Colaborador", b =>
                {
                    b.Navigation("ColaboradorFerias");
                });

            modelBuilder.Entity("ControleFerias.Models.Equipe", b =>
                {
                    b.Navigation("Colaboradores");
                });

            modelBuilder.Entity("ControleFerias.Models.Ferias", b =>
                {
                    b.Navigation("ColaboradorFerias");
                });
#pragma warning restore 612, 618
        }
    }
}
