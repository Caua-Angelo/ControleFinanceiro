using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Migrations
{
    /// <inheritdoc />
    public partial class inicialpostgre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colaborador_Equipe_EquipeId",
                schema: "public",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_ColaboradorFerias_Colaborador_ColaboradorId",
                schema: "public",
                table: "ColaboradorFerias");

            migrationBuilder.DropForeignKey(
                name: "FK_LogAlteracaoColaborador_Colaborador_ColaboradorId",
                schema: "public",
                table: "LogAlteracaoColaborador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colaborador",
                schema: "public",
                table: "Usuario");

            migrationBuilder.RenameTable(
                name: "Usuario",
                schema: "public",
                newName: "Usuario",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Colaborador_EquipeId",
                schema: "public",
                table: "Usuario",
                newName: "IX_colaborador_EquipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_colaborador",
                schema: "public",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_colaborador_Equipe_EquipeId",
                schema: "public",
                table: "Usuario",
                column: "EquipeId",
                principalSchema: "public",
                principalTable: "Equipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ColaboradorFerias_colaborador_ColaboradorId",
                schema: "public",
                table: "ColaboradorFerias",
                column: "ColaboradorId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogAlteracaoColaborador_colaborador_ColaboradorId",
                schema: "public",
                table: "LogAlteracaoColaborador",
                column: "ColaboradorId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_colaborador_Equipe_EquipeId",
                schema: "public",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_ColaboradorFerias_colaborador_ColaboradorId",
                schema: "public",
                table: "ColaboradorFerias");

            migrationBuilder.DropForeignKey(
                name: "FK_LogAlteracaoColaborador_colaborador_ColaboradorId",
                schema: "public",
                table: "LogAlteracaoColaborador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_colaborador",
                schema: "public",
                table: "Usuario");

            migrationBuilder.RenameTable(
                name: "Usuario",
                schema: "public",
                newName: "Usuario",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_colaborador_EquipeId",
                schema: "public",
                table: "Usuario",
                newName: "IX_Colaborador_EquipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colaborador",
                schema: "public",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Colaborador_Equipe_EquipeId",
                schema: "public",
                table: "Usuario",
                column: "EquipeId",
                principalSchema: "public",
                principalTable: "Equipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ColaboradorFerias_Colaborador_ColaboradorId",
                schema: "public",
                table: "ColaboradorFerias",
                column: "ColaboradorId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogAlteracaoColaborador_Colaborador_ColaboradorId",
                schema: "public",
                table: "LogAlteracaoColaborador",
                column: "ColaboradorId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
