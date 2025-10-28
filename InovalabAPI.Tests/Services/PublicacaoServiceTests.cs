using Xunit;
using FluentAssertions;
using InovalabAPI.Services;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InovalabAPI.Tests.Services
{
    public class PublicacaoServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly PublicacaoService _publicacaoService;
        private readonly Usuario _usuarioTeste;

        public PublicacaoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _publicacaoService = new PublicacaoService(_context);

            // Criar usuário de teste
            _usuarioTeste = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true,
                IsAdmin = true
            };
            _context.Usuarios.Add(_usuarioTeste);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDePublicacoes()
        {
            // Arrange
            var publicacoes = new List<Publicacao>
            {
                new Publicacao
                {
                    Titulo = "Publicacao 1",
                    Descricao = "Descricao da publicacao 1",
                    UsuarioId = _usuarioTeste.Id,
                    Status = "ativa",
                    CriadoEm = DateTime.UtcNow
                },
                new Publicacao
                {
                    Titulo = "Publicacao 2",
                    Descricao = "Descricao da publicacao 2",
                    UsuarioId = _usuarioTeste.Id,
                    Status = "ativa",
                    CriadoEm = DateTime.UtcNow
                }
            };
            _context.Publicacoes.AddRange(publicacoes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _publicacaoService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Titulo == "Publicacao 1");
            result.Should().Contain(p => p.Titulo == "Publicacao 2");
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarPublicacao()
        {
            // Arrange
            var publicacao = new Publicacao
            {
                Titulo = "Teste Publicacao",
                Descricao = "Descricao de teste",
                UsuarioId = _usuarioTeste.Id,
                Status = "ativa",
                CriadoEm = DateTime.UtcNow
            };
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            // Act
            var result = await _publicacaoService.GetByIdAsync(publicacao.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Titulo.Should().Be("Teste Publicacao");
        }

        [Fact]
        public async Task GetById_ComIdInvalido_DeveRetornarNull()
        {
            // Act
            var result = await _publicacaoService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveCriarPublicacao()
        {
            // Arrange
            var criarPublicacaoDto = new CriarPublicacaoDTO
            {
                Titulo = "Nova Publicacao",
                Descricao = "Descricao da nova publicacao"
            };

            // Act
            var result = await _publicacaoService.CreateAsync(criarPublicacaoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("Nova Publicacao");
            result.Descricao.Should().Be("Descricao da nova publicacao");

            var publicacaoNoBanco = await _context.Publicacoes.FindAsync(result.Id);
            publicacaoNoBanco.Should().NotBeNull();
            publicacaoNoBanco!.Titulo.Should().Be("Nova Publicacao");
        }

        [Fact]
        public async Task Update_ComDadosValidos_DeveAtualizarPublicacao()
        {
            // Arrange
            var publicacao = new Publicacao
            {
                Titulo = "Publicacao Original",
                Descricao = "Descricao original",
                UsuarioId = _usuarioTeste.Id,
                Status = "ativa",
                CriadoEm = DateTime.UtcNow
            };
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            var atualizarPublicacaoDto = new AtualizarPublicacaoDTO
            {
                Titulo = "Publicacao Atualizada",
                Descricao = "Descricao atualizado"
            };

            // Act
            var result = await _publicacaoService.UpdateAsync(publicacao.Id, atualizarPublicacaoDto);

            // Assert
            result.Should().BeTrue();

            var publicacaoAtualizada = await _context.Publicacoes.FindAsync(publicacao.Id);
            publicacaoAtualizada.Should().NotBeNull();
            publicacaoAtualizada!.Titulo.Should().Be("Publicacao Atualizada");
            publicacaoAtualizada.Descricao.Should().Be("Descricao atualizado");
        }

        [Fact]
        public async Task Delete_ComIdValido_DeveRemoverPublicacao()
        {
            // Arrange
            var publicacao = new Publicacao
            {
                Titulo = "Publicacao para Deletar",
                Descricao = "Descricao da publicacao",
                UsuarioId = _usuarioTeste.Id,
                Status = "ativa",
                CriadoEm = DateTime.UtcNow
            };
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            // Act
            var result = await _publicacaoService.DeleteAsync(publicacao.Id);

            // Assert
            result.Should().BeTrue();
            var publicacaoDeletada = await _context.Publicacoes.FindAsync(publicacao.Id);
            publicacaoDeletada.Should().BeNull();
        }

        [Fact]
        public async Task GetByStatus_ComStatusValido_DeveRetornarPublicacoes()
        {
            // Arrange
            var publicacoes = new List<Publicacao>
            {
                new Publicacao
                {
                    Titulo = "Publicacao Ativa 1",
                    Descricao = "Descricao 1",
                    UsuarioId = _usuarioTeste.Id,
                    Status = "ativa",
                    CriadoEm = DateTime.UtcNow
                },
                new Publicacao
                {
                    Titulo = "Publicacao Ativa 2",
                    Descricao = "Descricao 2",
                    UsuarioId = _usuarioTeste.Id,
                    Status = "ativa",
                    CriadoEm = DateTime.UtcNow
                },
                new Publicacao
                {
                    Titulo = "Publicacao Inativa",
                    Descricao = "Descricao 3",
                    UsuarioId = _usuarioTeste.Id,
                    Status = "rascunho",
                    CriadoEm = DateTime.UtcNow
                }
            };
            _context.Publicacoes.AddRange(publicacoes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _publicacaoService.GetByStatusAsync("ativa");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.Status == "ativa");
        }

        [Fact]
        public async Task AlterarStatus_DeveAlterarStatusDaPublicacao()
        {
            // Arrange
            var publicacao = new Publicacao
            {
                Titulo = "Publicacao Teste",
                Descricao = "Descricao teste",
                UsuarioId = _usuarioTeste.Id,
                Status = "ativa",
                CriadoEm = DateTime.UtcNow
            };
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            var alterarStatusDto = new AlterarStatusDTO
            {
                Status = "rascunho"
            };

            // Act
            var result = await _publicacaoService.AlterarStatusAsync(publicacao.Id, alterarStatusDto);

            // Assert
            result.Should().BeTrue();

            var publicacaoAtualizada = await _context.Publicacoes.FindAsync(publicacao.Id);
            publicacaoAtualizada.Should().NotBeNull();
            publicacaoAtualizada!.Status.Should().Be("rascunho");
        }

        [Fact]
        public async Task Create_ComTituloVazio_DeveCriarPublicacao()
        {
            // Arrange
            var criarPublicacaoDto = new CriarPublicacaoDTO
            {
                Titulo = "", // Título vazio
                Descricao = "Descricao da publicacao"
            };

            // Act
            var result = await _publicacaoService.CreateAsync(criarPublicacaoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("");
        }

        [Fact]
        public async Task Create_ComDescricaoVazia_DeveCriarPublicacao()
        {
            // Arrange
            var criarPublicacaoDto = new CriarPublicacaoDTO
            {
                Titulo = "Titulo da Publicacao",
                Descricao = "" // Descrição vazia
            };

            // Act
            var result = await _publicacaoService.CreateAsync(criarPublicacaoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Descricao.Should().Be("");
        }

        [Fact]
        public async Task Update_ComIdInvalido_DeveRetornarFalse()
        {
            // Arrange
            var atualizarPublicacaoDto = new AtualizarPublicacaoDTO
            {
                Titulo = "Titulo Atualizado",
                Descricao = "Descricao atualizada"
            };

            // Act
            var result = await _publicacaoService.UpdateAsync(999, atualizarPublicacaoDto);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_ComIdInvalido_DeveRetornarFalse()
        {
            // Act
            var result = await _publicacaoService.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetByStatus_ComStatusInvalido_DeveRetornarListaVazia()
        {
            // Act
            var result = await _publicacaoService.GetByStatusAsync("status-inexistente");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AlterarStatus_ComIdInvalido_DeveRetornarFalse()
        {
            // Arrange
            var alterarStatusDto = new AlterarStatusDTO
            {
                Status = "rascunho"
            };

            // Act
            var result = await _publicacaoService.AlterarStatusAsync(999, alterarStatusDto);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ComUsuarioIdInvalido_DeveLancarExcecao()
        {
            // Arrange
            var criarPublicacaoDto = new CriarPublicacaoDTO
            {
                Titulo = "Nova Publicacao",
                Descricao = "Descricao da nova publicacao"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _publicacaoService.CreateAsync(criarPublicacaoDto, 999)); // UsuarioId inválido
        }

        [Fact]
        public async Task GetAll_ComListaVazia_DeveRetornarListaVazia()
        {
            // Act
            var result = await _publicacaoService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Update_ComDadosNull_DeveAtualizarPublicacao()
        {
            // Arrange
            var publicacao = new Publicacao
            {
                Titulo = "Publicacao Original",
                Descricao = "Descricao original",
                UsuarioId = _usuarioTeste.Id,
                Status = "ativa",
                CriadoEm = DateTime.UtcNow
            };
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            var atualizarPublicacaoDto = new AtualizarPublicacaoDTO
            {
                Titulo = null!, // Título null
                Descricao = null! // Descrição null
            };

            // Act
            var result = await _publicacaoService.UpdateAsync(publicacao.Id, atualizarPublicacaoDto);

            // Assert
            result.Should().BeTrue();

            var publicacaoAtualizada = await _context.Publicacoes.FindAsync(publicacao.Id);
            publicacaoAtualizada.Should().NotBeNull();
            publicacaoAtualizada!.Titulo.Should().BeNull();
            publicacaoAtualizada.Descricao.Should().BeNull();
        }

        [Fact]
        public async Task GetByStatus_ComStatusNull_DeveRetornarListaVazia()
        {
            // Act
            var result = await _publicacaoService.GetByStatusAsync(null!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AlterarStatus_ComStatusNull_DeveAlterarStatus()
        {
            // Arrange
            var publicacao = new Publicacao
            {
                Titulo = "Publicacao Teste",
                Descricao = "Descricao teste",
                UsuarioId = _usuarioTeste.Id,
                Status = "ativa",
                CriadoEm = DateTime.UtcNow
            };
            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            var alterarStatusDto = new AlterarStatusDTO
            {
                Status = null! // Status null
            };

            // Act
            var result = await _publicacaoService.AlterarStatusAsync(publicacao.Id, alterarStatusDto);

            // Assert
            result.Should().BeTrue();

            var publicacaoAtualizada = await _context.Publicacoes.FindAsync(publicacao.Id);
            publicacaoAtualizada.Should().NotBeNull();
            publicacaoAtualizada!.Status.Should().BeNull();
        }
    }
}