using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAtributesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "public",
                table: "Usuario",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HashSenha",
                schema: "public",
                table: "Usuario",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "public",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "HashSenha",
                schema: "public",
                table: "Usuario");
        }
    }
}
