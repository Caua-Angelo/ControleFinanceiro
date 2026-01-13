using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFerias.Migrations
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
                table: "Colaborador");

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
                table: "Colaborador");

            migrationBuilder.RenameTable(
                name: "Colaborador",
                schema: "public",
                newName: "colaborador",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Colaborador_EquipeId",
                schema: "public",
                table: "colaborador",
                newName: "IX_colaborador_EquipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_colaborador",
                schema: "public",
                table: "colaborador",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_colaborador_Equipe_EquipeId",
                schema: "public",
                table: "colaborador",
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
                principalTable: "colaborador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogAlteracaoColaborador_colaborador_ColaboradorId",
                schema: "public",
                table: "LogAlteracaoColaborador",
                column: "ColaboradorId",
                principalSchema: "public",
                principalTable: "colaborador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_colaborador_Equipe_EquipeId",
                schema: "public",
                table: "colaborador");

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
                table: "colaborador");

            migrationBuilder.RenameTable(
                name: "colaborador",
                schema: "public",
                newName: "Colaborador",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_colaborador_EquipeId",
                schema: "public",
                table: "Colaborador",
                newName: "IX_Colaborador_EquipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colaborador",
                schema: "public",
                table: "Colaborador",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Colaborador_Equipe_EquipeId",
                schema: "public",
                table: "Colaborador",
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
                principalTable: "Colaborador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogAlteracaoColaborador_Colaborador_ColaboradorId",
                schema: "public",
                table: "LogAlteracaoColaborador",
                column: "ColaboradorId",
                principalSchema: "public",
                principalTable: "Colaborador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
