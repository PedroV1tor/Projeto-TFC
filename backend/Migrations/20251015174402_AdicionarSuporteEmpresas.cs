using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovalabAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSuporteEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Publicacoes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Publicacoes",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Orcamentos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Orcamentos",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Agendamentos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Agendamentos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publicacoes_EmpresaId",
                table: "Publicacoes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_EmpresaId",
                table: "Orcamentos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_EmpresaId",
                table: "Agendamentos",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Empresas_EmpresaId",
                table: "Agendamentos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Empresas_EmpresaId",
                table: "Orcamentos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicacoes_Empresas_EmpresaId",
                table: "Publicacoes",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Empresas_EmpresaId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Empresas_EmpresaId",
                table: "Orcamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Publicacoes_Empresas_EmpresaId",
                table: "Publicacoes");

            migrationBuilder.DropIndex(
                name: "IX_Publicacoes_EmpresaId",
                table: "Publicacoes");

            migrationBuilder.DropIndex(
                name: "IX_Orcamentos_EmpresaId",
                table: "Orcamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_EmpresaId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Publicacoes");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Agendamentos");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Publicacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Orcamentos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Agendamentos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
