using Microsoft.EntityFrameworkCore.Migrations;
using BCrypt.Net;

#nullable disable

namespace InovalabAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarioAdminPadrao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "Status",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Publicacoes");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Agendamentos");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

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

            // Inserir usuário admin padrão
            var senhaHash = BCrypt.Net.BCrypt.HashPassword("admin@123");
            var dataAtual = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            migrationBuilder.Sql($@"
                DO $$
                DECLARE
                    usuario_id INT;
                BEGIN
                    -- Primeiro, inserir o usuário admin
                    INSERT INTO ""Usuarios"" (""Nome"", ""Sobrenome"", ""Email"", ""NomeUsuario"", ""SenhaHash"", ""Telefone"", ""Ativo"", ""IsAdmin"", ""DataCriacao"")
                    VALUES ('Admin', 'Sistema', 'admin@inovalab.com', 'admin', '{senhaHash}', '(00) 00000-0000', true, true, '{dataAtual}')
                    ON CONFLICT (""Email"") DO NOTHING
                    RETURNING ""Id"" INTO usuario_id;
                    
                    -- Depois, inserir o endereço vinculado ao usuário
                    IF usuario_id IS NOT NULL THEN
                        INSERT INTO ""EnderecosUsuario"" (""UsuarioId"", ""CEP"", ""Rua"", ""Bairro"", ""Numero"", ""DataCriacao"")
                        VALUES (usuario_id, '01000-000', 'Rua da Administração', 'Centro', '100', '{dataAtual}');
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover usuário admin padrão
            migrationBuilder.Sql(@"
                DELETE FROM ""Usuarios"" WHERE ""Email"" = 'admin@inovalab.com';
                DELETE FROM ""EnderecosUsuario"" WHERE ""Rua"" = 'Rua da Administração' AND ""CEP"" = '01000-000';
            ");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pendente");

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

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Empresas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pendente");

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
    }
}
