using Xunit;
using Moq;
using FluentAssertions;
using InovalabAPI.Services;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InovalabAPI.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Configurar banco de dados em memória
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();

            // Configurar mock de configuração JWT
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["SecretKey"])
                .Returns("MinhaChaveSecretaSuperSeguraComMaisDe32Caracteres123456");
            jwtSectionMock.Setup(x => x["Issuer"])
                .Returns("InovalabAPI");
            jwtSectionMock.Setup(x => x["Audience"])
                .Returns("InovalabApp");
            jwtSectionMock.Setup(x => x["ExpiryInHours"])
                .Returns("24");

            _configurationMock.Setup(x => x.GetSection("JwtSettings"))
                .Returns(jwtSectionMock.Object);

            _authService = new AuthService(_context, _configurationMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
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
                IsAdmin = false
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("teste@email.com");
        }

        [Fact]
        public async Task Login_ComSenhaInvalida_DeveRetornarNull()
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

            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = "senhaErrada"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_ComEmailInexistente_DeveRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "inexistente@email.com",
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_ComUsuarioInativo_DeveRetornarNull()
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
                Ativo = false
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastro_ComDadosValidos_DeveCriarUsuario()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Novo",
                Sobrenome = "Usuario",
                Email = "novo@email.com",
                NomeUsuario = "novousuario",
                Senha = "senha123",
                Telefone = "(11) 98888-8888",
                Endereco = new EnderecoUsuarioDTO
                {
                    CEP = "01000-000",
                    Rua = "Rua Teste",
                    Bairro = "Centro",
                    Numero = "100"
                }
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue();

            var usuarioNoBanco = await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.Email == "novo@email.com");
            
            usuarioNoBanco.Should().NotBeNull();
            usuarioNoBanco!.Email.Should().Be("novo@email.com");
            usuarioNoBanco.Nome.Should().Be("Novo");
            usuarioNoBanco.Endereco.Should().NotBeNull();
            usuarioNoBanco.Endereco!.CEP.Should().Be("01000-000");
        }

        [Fact]
        public async Task Cadastro_ComEmailDuplicado_DeveRetornarFalse()
        {
            // Arrange
            var usuarioExistente = new Usuario
            {
                Nome = "Existente",
                Sobrenome = "Usuario",
                Email = "existente@email.com",
                NomeUsuario = "existenteusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuarioExistente);
            await _context.SaveChangesAsync();

            var cadastroRequest = new CadastroRequest
            {
                Nome = "Novo",
                Sobrenome = "Usuario",
                Email = "existente@email.com", // Email duplicado
                NomeUsuario = "novousuario",
                Senha = "senha123",
                Telefone = "(11) 98888-8888",
                Endereco = new EnderecoUsuarioDTO
                {
                    CEP = "01000-000",
                    Rua = "Rua Teste",
                    Bairro = "Centro",
                    Numero = "100"
                }
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task SolicitarRecuperacaoSenha_ComEmailValido_DeveEnviarEmail()
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

            _emailServiceMock.Setup(x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.SolicitarRecuperacaoSenhaAsync("teste@email.com");

            // Assert
            result.Should().BeTrue();
            _emailServiceMock.Verify(x => x.EnviarAsync(
                "teste@email.com",
                It.IsAny<string>(),
                It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public async Task SolicitarRecuperacaoSenha_ComEmailInexistente_DeveRetornarFalse()
        {
            // Act
            var result = await _authService.SolicitarRecuperacaoSenhaAsync("inexistente@email.com");

            // Assert
            result.Should().BeFalse();
            _emailServiceMock.Verify(x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), 
                Times.Never);
        }

        [Fact]
        public async Task Login_ComEmailVazio_DeveRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "",
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_ComSenhaVazia_DeveRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = ""
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_ComEmailNull_DeveRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = null!,
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_ComSenhaNull_DeveRetornarNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = null!
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastro_ComEmailVazio_DeveCriarUsuario()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "",
                NomeUsuario = "testusuario",
                Senha = "senha123",
                Telefone = "(11) 99999-9999"
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue(); // O serviço cria mesmo com email vazio
        }

        [Fact]
        public async Task Cadastro_ComSenhaMuitoCurta_DeveCriarUsuario()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                Senha = "123", // Senha muito curta
                Telefone = "(11) 99999-9999"
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue(); // O serviço cria mesmo com senha curta
        }

        [Fact]
        public async Task Cadastro_ComNomeUsuarioDuplicado_DeveRetornarFalse()
        {
            // Arrange
            var usuarioExistente = new Usuario
            {
                Nome = "Existente",
                Sobrenome = "Usuario",
                Email = "existente@email.com",
                NomeUsuario = "usuarioexistente",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuarioExistente);
            await _context.SaveChangesAsync();

            var cadastroRequest = new CadastroRequest
            {
                Nome = "Novo",
                Sobrenome = "Usuario",
                Email = "novo@email.com",
                NomeUsuario = "usuarioexistente", // Nome de usuário duplicado
                Senha = "senha123",
                Telefone = "(11) 98888-8888"
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task SolicitarRecuperacaoSenha_ComEmailVazio_DeveRetornarFalse()
        {
            // Act
            var result = await _authService.SolicitarRecuperacaoSenhaAsync("");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task SolicitarRecuperacaoSenha_ComEmailNull_DeveRetornarFalse()
        {
            // Act
            var result = await _authService.SolicitarRecuperacaoSenhaAsync(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Login_ComUsuarioAdmin_DeveRetornarTokenComIsAdminTrue()
        {
            // Arrange
            var usuarioAdmin = new Usuario
            {
                Nome = "Admin",
                Sobrenome = "Sistema",
                Email = "admin@inovalab.com",
                NomeUsuario = "admin",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                Telefone = "(00) 00000-0000",
                Ativo = true,
                IsAdmin = true
            };
            _context.Usuarios.Add(usuarioAdmin);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = "admin@inovalab.com",
                Senha = "admin@123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("admin@inovalab.com");
            result.IsAdmin.Should().BeTrue();
        }

        [Fact]
        public async Task Cadastro_ComEnderecoCompleto_DeveCriarUsuarioComEndereco()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Usuario",
                Sobrenome = "Completo",
                Email = "completo@email.com",
                NomeUsuario = "usuariocompleto",
                Senha = "senha123",
                Telefone = "(11) 99999-9999",
                Endereco = new EnderecoUsuarioDTO
                {
                    CEP = "01234-567",
                    Rua = "Rua das Flores",
                    Bairro = "Jardim das Rosas",
                    Numero = "123",
                    Complemento = "Apto 45",
                    Referencia = "Próximo ao shopping"
                }
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue();

            var usuarioNoBanco = await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.Email == "completo@email.com");
            
            usuarioNoBanco.Should().NotBeNull();
            usuarioNoBanco!.Endereco.Should().NotBeNull();
            usuarioNoBanco.Endereco!.CEP.Should().Be("01234-567");
            usuarioNoBanco.Endereco.Rua.Should().Be("Rua das Flores");
            usuarioNoBanco.Endereco.Bairro.Should().Be("Jardim das Rosas");
            usuarioNoBanco.Endereco.Numero.Should().Be("123");
            usuarioNoBanco.Endereco.Complemento.Should().Be("Apto 45");
            usuarioNoBanco.Endereco.Referencia.Should().Be("Próximo ao shopping");
        }

        [Fact]
        public async Task Cadastro_ComEnderecoMinimo_DeveCriarUsuarioComEnderecoBasico()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Usuario",
                Sobrenome = "Minimo",
                Email = "minimo@email.com",
                NomeUsuario = "usuariominimo",
                Senha = "senha123",
                Telefone = "(11) 99999-9999",
                Endereco = new EnderecoUsuarioDTO
                {
                    CEP = "01000-000",
                    Rua = "Rua Teste",
                    Bairro = "Centro"
                    // Numero, Complemento e Referencia são opcionais
                }
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue();

            var usuarioNoBanco = await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.Email == "minimo@email.com");
            
            usuarioNoBanco.Should().NotBeNull();
            usuarioNoBanco!.Endereco.Should().NotBeNull();
            usuarioNoBanco.Endereco!.CEP.Should().Be("01000-000");
            usuarioNoBanco.Endereco.Rua.Should().Be("Rua Teste");
            usuarioNoBanco.Endereco.Bairro.Should().Be("Centro");
        }

        [Fact]
        public async Task Login_ComSenhaComEspacos_DeveRetornarNull()
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

            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = " senha123 " // Senha com espaços
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_ComEmailComEspacos_DeveRetornarNull()
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

            var loginRequest = new LoginRequest
            {
                Email = " teste@email.com ", // Email com espaços
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Cadastro_ComTelefoneFormatoInvalido_DeveCriarUsuario()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                Senha = "senha123",
                Telefone = "telefone-invalido" // Telefone inválido
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue(); // O serviço cria mesmo com telefone inválido
        }

        [Fact]
        public async Task Cadastro_ComNomeUsuarioComEspacos_DeveCriarUsuario()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "usuario com espacos", // Nome de usuário com espaços
                Senha = "senha123",
                Telefone = "(11) 99999-9999"
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue(); // O serviço cria mesmo com espaços no nome de usuário
        }

        [Fact]
        public async Task SolicitarRecuperacaoSenha_ComEmailComEspacos_DeveRetornarFalse()
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
            var result = await _authService.SolicitarRecuperacaoSenhaAsync(" teste@email.com "); // Email com espaços

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Login_ComUsuarioComCaracteresEspeciais_DeveRetornarToken()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "João",
                Sobrenome = "Silva-Santos",
                Email = "joao.silva@email.com",
                NomeUsuario = "joao_silva",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha@123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = "joao.silva@email.com",
                Senha = "senha@123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("joao.silva@email.com");
        }

        [Fact]
        public async Task Cadastro_ComEnderecoNull_DeveCriarUsuario()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                Senha = "senha123",
                Telefone = "(11) 99999-9999"
                // Endereco não é definido (será null por padrão)
            };

            // Act
            var result = await _authService.CadastroAsync(cadastroRequest);

            // Assert
            result.Should().BeTrue(); // O serviço cria mesmo sem endereço
        }

        [Fact]
        public async Task Login_ComUsuarioComNomeMuitoLongo_DeveRetornarToken()
        {
            // Arrange
            var nomeLongo = new string('A', 100);
            var usuario = new Usuario
            {
                Nome = nomeLongo,
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = "senha123"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }
    }
}

