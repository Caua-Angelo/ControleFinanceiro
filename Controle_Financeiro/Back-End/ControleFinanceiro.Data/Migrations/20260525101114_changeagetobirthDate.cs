using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeagetobirthDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Idade",
                schema: "public",
                table: "Usuario");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                schema: "public",
                table: "Usuario",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataNascimento",
                schema: "public",
                table: "Usuario");

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                schema: "public",
                table: "Usuario",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
