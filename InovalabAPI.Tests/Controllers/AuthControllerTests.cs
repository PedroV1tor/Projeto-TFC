using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using InovalabAPI.Controllers;
using InovalabAPI.Services;
using InovalabAPI.Models.DTOs;
using Microsoft.Extensions.Configuration;

namespace InovalabAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarOk()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = "senha123"
            };

            var loginResponse = new LoginResponse
            {
                Token = "token123",
                Email = "teste@email.com",
                IsAdmin = false
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(loginResponse);
        }

        [Fact]
        public async Task Login_ComCredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = "senhaErrada"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Cadastro_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Novo",
                Sobrenome = "Usuario",
                Email = "novo@email.com",
                NomeUsuario = "novousuario",
                Senha = "senha123",
                Telefone = "(11) 98888-8888"
            };

            _authServiceMock.Setup(x => x.CadastroAsync(cadastroRequest))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Cadastro(cadastroRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(new { message = "Usuário cadastrado com sucesso" });
        }

        [Fact]
        public async Task Cadastro_ComEmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "Novo",
                Sobrenome = "Usuario",
                Email = "existente@email.com",
                NomeUsuario = "novousuario",
                Senha = "senha123",
                Telefone = "(11) 98888-8888"
            };

            _authServiceMock.Setup(x => x.CadastroAsync(cadastroRequest))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.Cadastro(cadastroRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { message = "Email ou nome de usuário já existe" });
        }

        [Fact]
        public async Task RecuperarSenha_ComEmailValido_DeveRetornarOk()
        {
            // Arrange
            var request = new RecuperarSenhaRequest
            {
                Email = "teste@email.com"
            };

            _authServiceMock.Setup(x => x.SolicitarRecuperacaoSenhaAsync(request.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.RecuperarSenha(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(new { message = "Código de verificação enviado para o email" });
        }

        [Fact]
        public async Task RecuperarSenha_ComEmailInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var request = new RecuperarSenhaRequest
            {
                Email = "inexistente@email.com"
            };

            _authServiceMock.Setup(x => x.SolicitarRecuperacaoSenhaAsync(request.Email))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.RecuperarSenha(request);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().BeEquivalentTo(new { message = "Email não encontrado" });
        }

        [Fact]
        public async Task Login_ComEmailVazio_DeveRetornarUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "",
                Senha = "senha123"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_ComSenhaVazia_DeveRetornarUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "teste@email.com",
                Senha = ""
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Cadastro_ComDadosVazios_DeveRetornarOk()
        {
            // Arrange
            var cadastroRequest = new CadastroRequest
            {
                Nome = "",
                Sobrenome = "",
                Email = "",
                NomeUsuario = "",
                Senha = "",
                Telefone = ""
            };

            _authServiceMock.Setup(x => x.CadastroAsync(cadastroRequest))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Cadastro(cadastroRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task RecuperarSenha_ComEmailVazio_DeveRetornarNotFound()
        {
            // Arrange
            var request = new RecuperarSenhaRequest
            {
                Email = ""
            };

            _authServiceMock.Setup(x => x.SolicitarRecuperacaoSenhaAsync(request.Email))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.RecuperarSenha(request);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
