using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovalabAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarStatusUsuarioEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pendente");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Empresas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pendente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Empresas");
        }
    }
}
