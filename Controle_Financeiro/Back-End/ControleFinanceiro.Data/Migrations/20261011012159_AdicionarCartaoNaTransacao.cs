using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCartaoNaTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartaoId",
                schema: "public",
                table: "Transacao",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacao_CartaoId",
                schema: "public",
                table: "Transacao",
                column: "CartaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacao_Cartao_CartaoId",
                schema: "public",
                table: "Transacao",
                column: "CartaoId",
                principalSchema: "public",
                principalTable: "Cartao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacao_Cartao_CartaoId",
                schema: "public",
                table: "Transacao");

            migrationBuilder.DropIndex(
                name: "IX_Transacao_CartaoId",
                schema: "public",
                table: "Transacao");

            migrationBuilder.DropColumn(
                name: "CartaoId",
                schema: "public",
                table: "Transacao");
        }
    }
}
