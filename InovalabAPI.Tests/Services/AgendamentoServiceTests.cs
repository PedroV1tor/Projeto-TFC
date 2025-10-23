using Xunit;
using FluentAssertions;
using InovalabAPI.Services;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InovalabAPI.Tests.Services
{
    public class AgendamentoServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AgendamentoService _agendamentoService;
        private readonly Usuario _usuarioTeste;

        public AgendamentoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _agendamentoService = new AgendamentoService(_context);

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
        public async Task GetAll_DeveRetornarListaDeAgendamentos()
        {
            // Arrange
            var agendamentos = new List<Agendamento>
            {
                new Agendamento
                {
                    Titulo = "Agendamento 1",
                    Descricao = "Descricao do agendamento 1",
                    Data = DateTime.UtcNow.AddDays(1),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Agendamento
                {
                    Titulo = "Agendamento 2",
                    Descricao = "Descricao do agendamento 2",
                    Data = DateTime.UtcNow.AddDays(2),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                }
            };
            _context.Agendamentos.AddRange(agendamentos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _agendamentoService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.Titulo == "Agendamento 1");
            result.Should().Contain(a => a.Titulo == "Agendamento 2");
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarAgendamento()
        {
            // Arrange
            var agendamento = new Agendamento
            {
                Titulo = "Teste Agendamento",
                Descricao = "Descricao de teste",
                Data = DateTime.UtcNow.AddDays(1),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            // Act
            var result = await _agendamentoService.GetByIdAsync(agendamento.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Titulo.Should().Be("Teste Agendamento");
        }

        [Fact]
        public async Task GetById_ComIdInvalido_DeveRetornarNull()
        {
            // Act
            var result = await _agendamentoService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveCriarAgendamento()
        {
            // Arrange
            var criarAgendamentoDto = new CriarAgendamentoDTO
            {
                Titulo = "Novo Agendamento",
                Descricao = "Descricao do novo agendamento",
                Data = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await _agendamentoService.CreateAsync(criarAgendamentoDto, _usuarioTeste.Id);

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("Novo Agendamento");
            result.Descricao.Should().Be("Descricao do novo agendamento");

            var agendamentoNoBanco = await _context.Agendamentos.FindAsync(result.Id);
            agendamentoNoBanco.Should().NotBeNull();
            agendamentoNoBanco!.Titulo.Should().Be("Novo Agendamento");
        }

        [Fact]
        public async Task Update_ComDadosValidos_DeveAtualizarAgendamento()
        {
            // Arrange
            var agendamento = new Agendamento
            {
                Titulo = "Agendamento Original",
                Descricao = "Descricao original",
                Data = DateTime.UtcNow.AddDays(1),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            var atualizarAgendamentoDto = new AtualizarAgendamentoDTO
            {
                Titulo = "Agendamento Atualizado",
                Descricao = "Descricao atualizada",
                Data = DateTime.UtcNow.AddDays(2)
            };

            // Act
            var result = await _agendamentoService.UpdateAsync(agendamento.Id, atualizarAgendamentoDto);

            // Assert
            result.Should().BeTrue();

            var agendamentoAtualizado = await _context.Agendamentos.FindAsync(agendamento.Id);
            agendamentoAtualizado.Should().NotBeNull();
            agendamentoAtualizado!.Titulo.Should().Be("Agendamento Atualizado");
            agendamentoAtualizado.Descricao.Should().Be("Descricao atualizada");
        }

        [Fact]
        public async Task Delete_ComIdValido_DeveRemoverAgendamento()
        {
            // Arrange
            var agendamento = new Agendamento
            {
                Titulo = "Agendamento para Deletar",
                Descricao = "Descricao do agendamento",
                Data = DateTime.UtcNow.AddDays(1),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            // Act
            var result = await _agendamentoService.DeleteAsync(agendamento.Id);

            // Assert
            result.Should().BeTrue();
            var agendamentoDeletado = await _context.Agendamentos.FindAsync(agendamento.Id);
            agendamentoDeletado.Should().BeNull();
        }

        [Fact]
        public async Task GetByStatus_ComStatusValido_DeveRetornarAgendamentos()
        {
            // Arrange
            var agendamentos = new List<Agendamento>
            {
                new Agendamento
                {
                    Titulo = "Agendamento Agendado 1",
                    Descricao = "Descricao 1",
                    Data = DateTime.UtcNow.AddDays(1),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Agendamento
                {
                    Titulo = "Agendamento Agendado 2",
                    Descricao = "Descricao 2",
                    Data = DateTime.UtcNow.AddDays(2),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Agendamento
                {
                    Titulo = "Agendamento Cancelado",
                    Descricao = "Descricao 3",
                    Data = DateTime.UtcNow.AddDays(3),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "reprovado"
                }
            };
            _context.Agendamentos.AddRange(agendamentos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _agendamentoService.GetByStatusAsync("pendente");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(a => a.Status == "pendente");
        }

        [Fact]
        public async Task GetByFiltro_ComPeriodoValido_DeveRetornarAgendamentos()
        {
            // Arrange
            var dataInicio = DateTime.UtcNow.AddDays(1);
            var dataFim = DateTime.UtcNow.AddDays(3);

            var agendamentos = new List<Agendamento>
            {
                new Agendamento
                {
                    Titulo = "Agendamento Dentro do Periodo",
                    Descricao = "Descricao 1",
                    Data = DateTime.UtcNow.AddDays(2),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                },
                new Agendamento
                {
                    Titulo = "Agendamento Fora do Periodo",
                    Descricao = "Descricao 2",
                    Data = DateTime.UtcNow.AddDays(5),
                    UsuarioId = _usuarioTeste.Id,
                    Status = "pendente"
                }
            };
            _context.Agendamentos.AddRange(agendamentos);
            await _context.SaveChangesAsync();

            var filtro = new FiltroAgendamentoDTO
            {
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            // Act
            var result = await _agendamentoService.GetByFiltroAsync(filtro);

            // Assert
            result.Should().HaveCount(1);
            result.Should().OnlyContain(a => a.Titulo == "Agendamento Dentro do Periodo");
        }

        [Fact]
        public async Task AlterarStatus_DeveAlterarStatusDoAgendamento()
        {
            // Arrange
            var agendamento = new Agendamento
            {
                Titulo = "Agendamento Teste",
                Descricao = "Descricao teste",
                Data = DateTime.UtcNow.AddDays(1),
                UsuarioId = _usuarioTeste.Id,
                Status = "pendente"
            };
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            var alterarStatusDto = new AlterarStatusAgendamentoDTO
            {
                Status = "aprovado"
            };

            // Act
            var result = await _agendamentoService.AlterarStatusAsync(agendamento.Id, alterarStatusDto);

            // Assert
            result.Should().BeTrue();

            var agendamentoAtualizado = await _context.Agendamentos.FindAsync(agendamento.Id);
            agendamentoAtualizado.Should().NotBeNull();
            agendamentoAtualizado!.Status.Should().Be("aprovado");
        }
    }
}