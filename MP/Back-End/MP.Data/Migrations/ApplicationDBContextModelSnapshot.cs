
using System;
using ControleFerias.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ControleFerias.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "8.0.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ControleFerias.Models.Colaborador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EquipeId")
                        .HasColumnType("integer");

                    b.Property<string>("sNome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EquipeId");

                    b.ToTable("Colaborador", "public");
                });

            modelBuilder.Entity("ControleFerias.Models.ColaboradorFerias", b =>
                {
                    b.Property<int>("FeriasId")
                        .HasColumnType("integer");

                    b.Property<int>("ColaboradorId")
                        .HasColumnType("integer");

                    b.HasKey("FeriasId", "ColaboradorId");

                    b.HasIndex("ColaboradorId");

                    b.ToTable("ColaboradorFerias", "public");
                });

            modelBuilder.Entity("ControleFerias.Models.Equipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("sNome")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Equipe", "public");
                });

            modelBuilder.Entity("ControleFerias.Models.Ferias", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("dDataFinal")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("dDataInicio")
                        .HasColumnType("timestamp");

                    b.Property<string>("sComentario")
                        .HasColumnType("text");

                    b.Property<int>("sDias")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Ferias", "public");
                });

            modelBuilder.Entity("ControleFerias.Models.LogAlteracaoColaborador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ColaboradorId")
                        .HasColumnType("integer");

                    b.Property<int>("EquipeId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ddataAlteracao")
                        .HasColumnType("timestamp");

                    b.Property<string>("sNome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ColaboradorId");

                    b.HasIndex("EquipeId");

                    b.ToTable("LogAlteracaoColaborador", "public");
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

            modelBuilder.Entity("ControleFerias.Models.LogAlteracaoColaborador", b =>
                {
                    b.HasOne("ControleFerias.Models.Colaborador", null)
                        .WithMany()
                        .HasForeignKey("ColaboradorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ControleFerias.Models.Equipe", null)
                        .WithMany()
                        .HasForeignKey("EquipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
