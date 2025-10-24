using Xunit;
using FluentAssertions;
using InovalabAPI.Services;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InovalabAPI.Tests.Services
{
    public class OrcamentoServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly OrcamentoService _orcamentoService;
        private readonly Usuario _usuarioTeste;

        public OrcamentoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _orcamentoService = new OrcamentoService(_context);

            // Criar usuário de teste
            _usuarioTeste = new Usuario
            {
                Nome = "Teste",
                Sobrenome = "Usuario",
                Email = "teste@email.com",
                NomeUsuario = "testusuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 99999-9999",
                Ativo = true
            };
            _context.Usuarios.Add(_usuarioTeste);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDeOrcamentos()
        {
            // Arrange
            var orcamentos = new List<Orcamento>
            {
                new Orcamento
                {
                    Titulo = "Orcamento 1",
                    Descricao = "Descricao do orcamento 1",
                    Valor = 1000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Orcamento
                {
                    Titulo = "Orcamento 2",
                    Descricao = "Descricao do orcamento 2",
                    Valor = 2000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                }
            };
            _context.Orcamentos.AddRange(orcamentos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orcamentoService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(o => o.Titulo == "Orcamento 1");
            result.Should().Contain(o => o.Titulo == "Orcamento 2");
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOrcamento()
        {
            // Arrange
            var orcamento = new Orcamento
            {
                Titulo = "Teste Orcamento",
                Descricao = "Descricao de teste",
                Valor = 1500.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orcamentoService.GetByIdAsync(orcamento.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Titulo.Should().Be("Teste Orcamento");
        }

        [Fact]
        public async Task GetById_ComIdInvalido_DeveRetornarNull()
        {
            // Act
            var result = await _orcamentoService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveCriarOrcamento()
        {
            // Arrange
            var criarOrcamentoDto = new CriarOrcamentoDTO
            {
                Titulo = "Novo Orcamento",
                Descricao = "Descricao do novo orcamento",
                Valor = 3000.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await _orcamentoService.CreateAsync(criarOrcamentoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("Novo Orcamento");
            result.Descricao.Should().Be("Descricao do novo orcamento");
            result.Valor.Should().Be(3000.00m);

            var orcamentoNoBanco = await _context.Orcamentos.FindAsync(result.Id);
            orcamentoNoBanco.Should().NotBeNull();
            orcamentoNoBanco!.Titulo.Should().Be("Novo Orcamento");
        }

        [Fact]
        public async Task Update_ComDadosValidos_DeveAtualizarOrcamento()
        {
            // Arrange
            var orcamento = new Orcamento
            {
                Titulo = "Orcamento Original",
                Descricao = "Descricao original",
                Valor = 1000.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            var atualizarOrcamentoDto = new AtualizarOrcamentoDTO
            {
                Titulo = "Orcamento Atualizado",
                Descricao = "Descricao atualizada",
                Valor = 1500.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(45),
                PrazoOrcamento = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var result = await _orcamentoService.UpdateAsync(orcamento.Id, atualizarOrcamentoDto);

            // Assert
            result.Should().BeTrue();

            var orcamentoAtualizado = await _context.Orcamentos.FindAsync(orcamento.Id);
            orcamentoAtualizado.Should().NotBeNull();
            orcamentoAtualizado!.Titulo.Should().Be("Orcamento Atualizado");
            orcamentoAtualizado.Descricao.Should().Be("Descricao atualizada");
            orcamentoAtualizado.Valor.Should().Be(1500.00m);
        }

        [Fact]
        public async Task Delete_ComIdValido_DeveRemoverOrcamento()
        {
            // Arrange
            var orcamento = new Orcamento
            {
                Titulo = "Orcamento para Deletar",
                Descricao = "Descricao do orcamento",
                Valor = 2000.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orcamentoService.DeleteAsync(orcamento.Id);

            // Assert
            result.Should().BeTrue();
            var orcamentoDeletado = await _context.Orcamentos.FindAsync(orcamento.Id);
            orcamentoDeletado.Should().BeNull();
        }

        [Fact]
        public async Task GetByStatus_ComStatusValido_DeveRetornarOrcamentos()
        {
            // Arrange
            var orcamentos = new List<Orcamento>
            {
                new Orcamento
                {
                    Titulo = "Orcamento Pendente 1",
                    Descricao = "Descricao 1",
                    Valor = 1000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Orcamento
                {
                    Titulo = "Orcamento Pendente 2",
                    Descricao = "Descricao 2",
                    Valor = 2000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Orcamento
                {
                    Titulo = "Orcamento Aprovado",
                    Descricao = "Descricao 3",
                    Valor = 3000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "aprovado"
                }
            };
            _context.Orcamentos.AddRange(orcamentos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orcamentoService.GetByStatusAsync("pendente");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(o => o.Status == "pendente");
        }

        [Fact]
        public async Task GetByFiltro_ComValorMinimo_DeveRetornarOrcamentos()
        {
            // Arrange
            var orcamentos = new List<Orcamento>
            {
                new Orcamento
                {
                    Titulo = "Orcamento Alto",
                    Descricao = "Descricao 1",
                    Valor = 5000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Orcamento
                {
                    Titulo = "Orcamento Baixo",
                    Descricao = "Descricao 2",
                    Valor = 1000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                }
            };
            _context.Orcamentos.AddRange(orcamentos);
            await _context.SaveChangesAsync();

            var filtro = new FiltroOrcamentoDTO
            {
                ValorMinimo = 2000.00m
            };

            // Act
            var result = await _orcamentoService.GetByFiltroAsync(filtro);

            // Assert
            result.Should().HaveCount(1);
            result.Should().OnlyContain(o => o.Titulo == "Orcamento Alto");
        }

        [Fact]
        public async Task AlterarStatus_DeveAlterarStatusDoOrcamento()
        {
            // Arrange
            var orcamento = new Orcamento
            {
                Titulo = "Orcamento Teste",
                Descricao = "Descricao teste",
                Valor = 2500.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            var alterarStatusDto = new AlterarStatusOrcamentoDTO
            {
                Status = "aprovado"
            };

            // Act
            var result = await _orcamentoService.AlterarStatusAsync(orcamento.Id, alterarStatusDto);

            // Assert
            result.Should().BeTrue();

            var orcamentoAtualizado = await _context.Orcamentos.FindAsync(orcamento.Id);
            orcamentoAtualizado.Should().NotBeNull();
            orcamentoAtualizado!.Status.Should().Be("aprovado");
        }

        [Fact]
        public async Task GetByUsuario_ComUsuarioValido_DeveRetornarOrcamentos()
        {
            // Arrange
            var outroUsuario = new Usuario
            {
                Nome = "Outro",
                Sobrenome = "Usuario",
                Email = "outro@email.com",
                NomeUsuario = "outrousuario",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Telefone = "(11) 88888-8888",
                Ativo = true
            };
            _context.Usuarios.Add(outroUsuario);
            await _context.SaveChangesAsync();

            var orcamentos = new List<Orcamento>
            {
                new Orcamento
                {
                    Titulo = "Orcamento Usuario Teste",
                    Descricao = "Descricao 1",
                    Valor = 1000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Orcamento
                {
                    Titulo = "Orcamento Outro Usuario",
                    Descricao = "Descricao 2",
                    Valor = 2000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = outroUsuario.Id,
                    Status = "pendente"
                }
            };
            _context.Orcamentos.AddRange(orcamentos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orcamentoService.GetByUsuarioAsync(_usuarioTeste.Id);

            // Assert
            result.Should().HaveCount(1);
            result.Should().OnlyContain(o => o.Titulo == "Orcamento Usuario Teste");
        }

        [Fact]
        public async Task Create_ComValorZero_DeveCriarOrcamento()
        {
            // Arrange
            var criarOrcamentoDto = new CriarOrcamentoDTO
            {
                Titulo = "Orcamento Zero",
                Descricao = "Descricao do orcamento",
                Valor = 0.00m, // Valor zero
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await _orcamentoService.CreateAsync(criarOrcamentoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Valor.Should().Be(0.00m);
        }

        [Fact]
        public async Task Create_ComValorNegativo_DeveCriarOrcamento()
        {
            // Arrange
            var criarOrcamentoDto = new CriarOrcamentoDTO
            {
                Titulo = "Orcamento Negativo",
                Descricao = "Descricao do orcamento",
                Valor = -100.00m, // Valor negativo
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await _orcamentoService.CreateAsync(criarOrcamentoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Valor.Should().Be(-100.00m);
        }

        [Fact]
        public async Task Update_ComIdInvalido_DeveRetornarFalse()
        {
            // Arrange
            var atualizarOrcamentoDto = new AtualizarOrcamentoDTO
            {
                Titulo = "Titulo Atualizado",
                Descricao = "Descricao atualizada",
                Valor = 1500.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(45),
                PrazoOrcamento = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var result = await _orcamentoService.UpdateAsync(999, atualizarOrcamentoDto);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_ComIdInvalido_DeveRetornarFalse()
        {
            // Act
            var result = await _orcamentoService.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetByStatus_ComStatusInvalido_DeveRetornarListaVazia()
        {
            // Act
            var result = await _orcamentoService.GetByStatusAsync("status-inexistente");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AlterarStatus_ComIdInvalido_DeveRetornarFalse()
        {
            // Arrange
            var alterarStatusDto = new AlterarStatusOrcamentoDTO
            {
                Status = "aprovado"
            };

            // Act
            var result = await _orcamentoService.AlterarStatusAsync(999, alterarStatusDto);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetByFiltro_ComValorMaximo_DeveRetornarOrcamentos()
        {
            // Arrange
            var orcamentos = new List<Orcamento>
            {
                new Orcamento
                {
                    Titulo = "Orcamento Alto",
                    Descricao = "Descricao 1",
                    Valor = 5000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Orcamento
                {
                    Titulo = "Orcamento Baixo",
                    Descricao = "Descricao 2",
                    Valor = 1000.00m,
                    PrazoEntrega = DateTime.UtcNow.AddDays(30),
                    PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                }
            };
            _context.Orcamentos.AddRange(orcamentos);
            await _context.SaveChangesAsync();

            var filtro = new FiltroOrcamentoDTO
            {
                ValorMaximo = 2000.00m
            };

            // Act
            var result = await _orcamentoService.GetByFiltroAsync(filtro);

            // Assert
            result.Should().HaveCount(1);
            result.Should().OnlyContain(o => o.Titulo == "Orcamento Baixo");
        }

        [Fact]
        public async Task GetAll_ComListaVazia_DeveRetornarListaVazia()
        {
            // Act
            var result = await _orcamentoService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Update_ComDadosNull_DeveAtualizarOrcamento()
        {
            // Arrange
            var orcamento = new Orcamento
            {
                Titulo = "Orcamento Original",
                Descricao = "Descricao original",
                Valor = 1000.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            var atualizarOrcamentoDto = new AtualizarOrcamentoDTO
            {
                Titulo = null!, // Título null
                Descricao = null! // Descrição null
            };

            // Act
            var result = await _orcamentoService.UpdateAsync(orcamento.Id, atualizarOrcamentoDto);

            // Assert
            result.Should().BeTrue();

            var orcamentoAtualizado = await _context.Orcamentos.FindAsync(orcamento.Id);
            orcamentoAtualizado.Should().NotBeNull();
            orcamentoAtualizado!.Titulo.Should().BeNull();
            orcamentoAtualizado.Descricao.Should().BeNull();
        }

        [Fact]
        public async Task GetByStatus_ComStatusNull_DeveRetornarListaVazia()
        {
            // Act
            var result = await _orcamentoService.GetByStatusAsync(""); // Usar string vazia em vez de null

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AlterarStatus_ComStatusVazio_DeveAlterarStatus()
        {
            // Arrange
            var orcamento = new Orcamento
            {
                Titulo = "Orcamento Teste",
                Descricao = "Descricao teste",
                Valor = 2500.00m,
                PrazoEntrega = DateTime.UtcNow.AddDays(30),
                PrazoOrcamento = DateTime.UtcNow.AddDays(7),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            var alterarStatusDto = new AlterarStatusOrcamentoDTO
            {
                Status = "" // Status vazio em vez de null
            };

            // Act
            var result = await _orcamentoService.AlterarStatusAsync(orcamento.Id, alterarStatusDto);

            // Assert
            result.Should().BeTrue();

            var orcamentoAtualizado = await _context.Orcamentos.FindAsync(orcamento.Id);
            orcamentoAtualizado.Should().NotBeNull();
            orcamentoAtualizado!.Status.Should().Be("");
        }
    }
}