using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infra.Data.Migrations
{
    public partial class AdicionarCategoriaCompraParcelada : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A coluna CategoriaId já existe no banco.
            // A compra existente já está associada à categoria 12.

            migrationBuilder.CreateIndex(
                name: "IX_CompraParcelada_CategoriaId",
                schema: "public",
                table: "CompraParcelada",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompraParcelada_Categoria_CategoriaId",
                schema: "public",
                table: "CompraParcelada",
                column: "CategoriaId",
                principalSchema: "public",
                principalTable: "Categoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompraParcelada_Categoria_CategoriaId",
                schema: "public",
                table: "CompraParcelada");

            migrationBuilder.DropIndex(
                name: "IX_CompraParcelada_CategoriaId",
                schema: "public",
                table: "CompraParcelada");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                schema: "public",
                table: "CompraParcelada");
        }
    }
}
