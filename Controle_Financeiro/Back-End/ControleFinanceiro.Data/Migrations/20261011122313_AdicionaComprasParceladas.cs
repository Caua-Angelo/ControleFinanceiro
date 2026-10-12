using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ControleFinanceiro.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaComprasParceladas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompraParcelada",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "integer", nullable: false),
                    DataCompra = table.Column<DateOnly>(type: "date", nullable: false),
                    CartaoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraParcelada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompraParcelada_Cartao_CartaoId",
                        column: x => x.CartaoId,
                        principalSchema: "public",
                        principalTable: "Cartao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompraParcelada_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "public",
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parcela",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Paga = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CompraParceladaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcela", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcela_CompraParcelada_CompraParceladaId",
                        column: x => x.CompraParceladaId,
                        principalSchema: "public",
                        principalTable: "CompraParcelada",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompraParcelada_CartaoId",
                schema: "public",
                table: "CompraParcelada",
                column: "CartaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraParcelada_UsuarioId",
                schema: "public",
                table: "CompraParcelada",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcela_CompraParceladaId_Numero",
                schema: "public",
                table: "Parcela",
                columns: new[] { "CompraParceladaId", "Numero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcela",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CompraParcelada",
                schema: "public");
        }
    }
}
