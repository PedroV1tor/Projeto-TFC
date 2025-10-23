using Xunit;
using FluentAssertions;
using InovalabAPI.Services;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InovalabAPI.Tests.Services
{
    public class UserServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _userService = new UserService(_context);
        }

        [Fact]
        public async Task GetAllUsuarios_DeveRetornarListaDeUsuarios()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Nome = "Usuario1",
                    Sobrenome = "Teste1",
                    Email = "usuario1@email.com",
                    NomeUsuario = "usuario1",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                    Telefone = "(11) 99999-9991",
                    Ativo = true
                },
                new Usuario
                {
                    Nome = "Usuario2",
                    Sobrenome = "Teste2",
                    Email = "usuario2@email.com",
                    NomeUsuario = "usuario2",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                    Telefone = "(11) 99999-9992",
                    Ativo = true
                }
            };
            _context.Usuarios.AddRange(usuarios);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetAllUsuariosAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Email == "usuario1@email.com");
            result.Should().Contain(u => u.Email == "usuario2@email.com");
        }

        [Fact]
        public async Task GetUsuarioById_ComIdValido_DeveRetornarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUsuarioByIdAsync(usuario.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("teste@email.com");
            result.Nome.Should().Be("Teste");
        }

        [Fact]
        public async Task GetUsuarioById_ComIdInvalido_DeveRetornarNull()
        {
            // Act
            var result = await _userService.GetUsuarioByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUsuarioByEmail_ComEmailValido_DeveRetornarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUsuarioByEmailAsync("teste@email.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("teste@email.com");
        }

        [Fact]
        public async Task UpdateUsuario_ComDadosValidos_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true,
                Endereco = new EnderecoUsuario
                {
                    CEP = "01000-000",
                    Rua = "Rua Antiga",
                    Bairro = "Centro",
                    Numero = "100"
                }
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Modificar dados do usuário
            usuario.Nome = "Teste Atualizado";
            usuario.Sobrenome = "Usuario Atualizado";
            usuario.Telefone = "(11) 98888-8888";
            usuario.Endereco!.Rua = "Rua Nova";
            usuario.Endereco.Bairro = "Jardim";

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeTrue();
            
            var usuarioAtualizado = await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.Id == usuario.Id);
            
            usuarioAtualizado.Should().NotBeNull();
            usuarioAtualizado!.Nome.Should().Be("Teste Atualizado");
            usuarioAtualizado.Sobrenome.Should().Be("Usuario Atualizado");
            usuarioAtualizado.Telefone.Should().Be("(11) 98888-8888");
            usuarioAtualizado.Endereco.Should().NotBeNull();
            usuarioAtualizado.Endereco!.Rua.Should().Be("Rua Nova");
        }

        [Fact]
        public async Task DeleteUsuario_ComIdValido_DeveRemoverUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.DeleteUsuarioAsync(usuario.Id);

            // Assert
            result.Should().BeTrue();
            var usuarioDeletado = await _context.Usuarios.FindAsync(usuario.Id);
            usuarioDeletado.Should().NotBeNull();
            usuarioDeletado!.Ativo.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteUsuario_ComIdInvalido_DeveRetornarFalse()
        {
            // Act
            var result = await _userService.DeleteUsuarioAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateUsuario_ComIdInvalido_DeveRetornarFalse()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 999, // ID inválido
                Nome = "Nome Atualizado",
                Sobrenome = "Sobrenome Atualizado",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 88888-8888",
                Ativo = true
            };

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateUsuario_ComDadosVazios_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Modificar para dados vazios
            usuario.Nome = "";
            usuario.Sobrenome = "";
            usuario.Telefone = "";

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeTrue(); // O serviço atualiza mesmo com dados vazios
        }

        [Fact]
        public async Task UpdateUsuario_ComDadosNull_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Modificar para dados null
            usuario.Nome = null!;
            usuario.Sobrenome = null!;
            usuario.Telefone = null!;

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeTrue(); // O serviço atualiza mesmo com dados null
        }

        [Fact]
        public async Task GetUsuarioByEmail_ComEmailVazio_DeveRetornarNull()
        {
            // Act
            var result = await _userService.GetUsuarioByEmailAsync("");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUsuarioByEmail_ComEmailNull_DeveRetornarNull()
        {
            // Act
            var result = await _userService.GetUsuarioByEmailAsync(null!);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUsuarioByEmail_ComEmailFormatoInvalido_DeveRetornarNull()
        {
            // Act
            var result = await _userService.GetUsuarioByEmailAsync("email-invalido");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllUsuarios_ComListaVazia_DeveRetornarListaVazia()
        {
            // Act
            var result = await _userService.GetAllUsuariosAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUsuarios_ComUsuariosInativos_DeveRetornarApenasUsuariosAtivos()
        {
            // Arrange - Criar um contexto isolado para este teste
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var isolatedContext = new ApplicationDbContext(options);
            var isolatedUserService = new UserService(isolatedContext);

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Nome = "Usuario",
                    Sobrenome = "Ativo",
                    Email = "ativo3@email.com", // Email único
                    NomeUsuario = "usuarioativo3",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                    Telefone = "(11) 99999-9991",
                    Ativo = true
                },
                new Usuario
                {
                    Nome = "Usuario",
                    Sobrenome = "Inativo",
                    Email = "inativo3@email.com", // Email único
                    NomeUsuario = "usuarioinativo3",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                    Telefone = "(11) 99999-9992",
                    Ativo = false
                }
            };
            isolatedContext.Usuarios.AddRange(usuarios);
            await isolatedContext.SaveChangesAsync();

            // Act
            var result = await isolatedUserService.GetAllUsuariosAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1); // Apenas usuários ativos são retornados
            result.Should().Contain(u => u.Email == "ativo3@email.com");
            result.Should().NotContain(u => u.Email == "inativo3@email.com"); // Usuário inativo não deve aparecer
        }

        [Fact]
        public async Task UpdateUsuario_ComTelefoneFormatoInvalido_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Modificar para telefone inválido
            usuario.Telefone = "telefone-invalido";

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeTrue(); // O serviço atualiza mesmo com formato inválido
        }

        [Fact]
        public async Task UpdateUsuario_ComNomeMuitoLongo_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Modificar para nome muito longo
            usuario.Nome = new string('A', 101); // Nome muito longo (mais de 100 caracteres)

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeTrue(); // O serviço atualiza mesmo com nome muito longo
        }

        [Fact]
        public async Task DeleteUsuario_ComUsuarioJaInativo_DeveRetornarFalse()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = false // Já inativo
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.DeleteUsuarioAsync(usuario.Id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUsuarioById_ComIdZero_DeveRetornarNull()
        {
            // Act
            var result = await _userService.GetUsuarioByIdAsync(0);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUsuarioById_ComIdNegativo_DeveRetornarNull()
        {
            // Act
            var result = await _userService.GetUsuarioByIdAsync(-1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateUsuario_ComIdZero_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 0, // ID zero
                Nome = "Nome Atualizado",
                Sobrenome = "Sobrenome Atualizado",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 88888-8888",
                Ativo = true
            };

            // Act
            var result = await _userService.UpdateUsuarioAsync(usuario);

            // Assert
            result.Should().BeTrue(); // O serviço atualiza mesmo com ID zero
        }

        [Fact]
        public async Task DeleteUsuario_ComIdZero_DeveRetornarFalse()
        {
            // Act
            var result = await _userService.DeleteUsuarioAsync(0);

            // Assert
            result.Should().BeFalse();
        }
    }
}

