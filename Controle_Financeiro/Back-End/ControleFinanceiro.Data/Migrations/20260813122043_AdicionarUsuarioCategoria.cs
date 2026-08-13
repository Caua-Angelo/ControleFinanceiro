using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarioCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                schema: "public",
                table: "Categoria",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_UsuarioId",
                schema: "public",
                table: "Categoria",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_Usuario_UsuarioId",
                schema: "public",
                table: "Categoria",
                column: "UsuarioId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_Usuario_UsuarioId",
                schema: "public",
                table: "Categoria");

            migrationBuilder.DropIndex(
                name: "IX_Categoria_UsuarioId",
                schema: "public",
                table: "Categoria");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                schema: "public",
                table: "Categoria");
        }
    }
}
