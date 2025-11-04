using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InovalabAPI.Services;
using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelatorioController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPublicacaoService _publicacaoService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IOrcamentoService _orcamentoService;

        public RelatorioController(
            IUserService userService,
            IPublicacaoService publicacaoService,
            IAgendamentoService agendamentoService,
            IOrcamentoService orcamentoService)
        {
            _userService = userService;
            _publicacaoService = publicacaoService;
            _agendamentoService = agendamentoService;
            _orcamentoService = orcamentoService;
        }

        [HttpGet("resumo")]
        public async Task<ActionResult<RelatorioResumoDTO>> GetRelatorioResumo(
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            try
            {


                var usuarios = await _userService.GetAllUsuariosAsync();
                var publicacoes = await _publicacaoService.GetAllAsync();
                var agendamentos = await _agendamentoService.GetAllAsync();
                var orcamentos = await _orcamentoService.GetAllAsync();

                var usuariosFiltrados = usuarios.AsQueryable();
                var publicacoesFiltradas = publicacoes.AsQueryable();
                var agendamentosFiltrados = agendamentos.AsQueryable();
                var orcamentosFiltrados = orcamentos.AsQueryable();

                if (dataInicial.HasValue)
                {
                    usuariosFiltrados = usuariosFiltrados.Where(u => u.DataCriacao >= dataInicial.Value);
                    publicacoesFiltradas = publicacoesFiltradas.Where(p => p.CriadoEm >= dataInicial.Value);
                    agendamentosFiltrados = agendamentosFiltrados.Where(a => a.CriadoEm >= dataInicial.Value);
                    orcamentosFiltrados = orcamentosFiltrados.Where(o => o.CriadoEm >= dataInicial.Value);
                }

                if (dataFinal.HasValue)
                {
                    usuariosFiltrados = usuariosFiltrados.Where(u => u.DataCriacao <= dataFinal.Value);
                    publicacoesFiltradas = publicacoesFiltradas.Where(p => p.CriadoEm <= dataFinal.Value);
                    agendamentosFiltrados = agendamentosFiltrados.Where(a => a.CriadoEm <= dataFinal.Value);
                    orcamentosFiltrados = orcamentosFiltrados.Where(o => o.CriadoEm <= dataFinal.Value);
                }

                var resumo = new RelatorioResumoDTO
                {
                    TotalUsuarios = usuariosFiltrados.Count(),
                    UsuariosAtivos = usuariosFiltrados.Count(),
                    TotalPublicacoes = publicacoesFiltradas.Count(),
                    PublicacoesAtivas = publicacoesFiltradas.Count(p => p.Status == "ativa"),
                    TotalAgendamentos = agendamentosFiltrados.Count(),
                    AgendamentosAtivos = agendamentosFiltrados.Count(a => a.Status == "aprovado"),
                    TotalOrcamentos = orcamentosFiltrados.Count(),
                    OrcamentosAprovados = orcamentosFiltrados.Count(o => o.Status == "aprovado"),
                    ValorTotalOrcamentos = orcamentosFiltrados.Sum(o => (decimal?)o.Valor) ?? 0m,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    GeradoEm = DateTime.Now
                };

                return Ok(resumo);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("usuarios")]
        public async Task<ActionResult<RelatorioResponseDTO<RelatorioUsuarioDTO>>> GetRelatorioUsuarios(
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            try
            {


                var usuarios = await _userService.GetAllUsuariosAsync();
                var usuariosList = usuarios.ToList();

                if (usuariosList.Any())
                {

                    foreach (var u in usuariosList.Take(3))
                    {
                    }
                }

                if (dataInicial.HasValue || dataFinal.HasValue)
                {

                    var usuariosAntesDoFiltro = usuariosList.Count;

                    usuariosList = usuariosList.Where(u =>
                    {
                        var dataCriacao = u.DataCriacao.Date;
                        var incluir = true;

                        if (dataInicial.HasValue)
                        {
                            incluir = incluir && dataCriacao >= dataInicial.Value.Date;
                        }

                        if (dataFinal.HasValue)
                        {
                            incluir = incluir && dataCriacao <= dataFinal.Value.Date;
                        }

                        return incluir;
                    }).ToList();
                }
                else
                {

                }

                var relatorioUsuarios = usuariosList.Select(u => new RelatorioUsuarioDTO
                {
                    Id = u.Id,
                    Nome = $"{u.Nome} {u.Sobrenome}",
                    Email = u.Email,
                    Telefone = u.Telefone ?? "Não informado",
                    Status = u.UltimoLogin.HasValue && u.UltimoLogin.Value > DateTime.Now.AddMonths(-3) ? "Ativo" : "Inativo",
                    CriadoEm = u.DataCriacao
                }).ToList();

                var response = new RelatorioResponseDTO<RelatorioUsuarioDTO>
                {
                    Total = relatorioUsuarios.Count,
                    Items = relatorioUsuarios,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    GeradoEm = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("publicacoes")]
        public async Task<ActionResult<RelatorioResponseDTO<RelatorioPublicacaoDTO>>> GetRelatorioPublicacoes(
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            try
            {
                var publicacoes = await _publicacaoService.GetAllAsync();
                var publicacoesList = publicacoes.ToList();

                if (dataInicial.HasValue || dataFinal.HasValue)
                {
                    publicacoesList = publicacoesList.Where(p =>
                    {
                        var dataCriacao = p.CriadoEm.Date;
                        var incluir = true;

                        if (dataInicial.HasValue)
                            incluir = incluir && dataCriacao >= dataInicial.Value.Date;

                        if (dataFinal.HasValue)
                            incluir = incluir && dataCriacao <= dataFinal.Value.Date;

                        return incluir;
                    }).ToList();
                }

                var relatorioPublicacoes = publicacoesList.Select(p => new RelatorioPublicacaoDTO
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Autor = p.Autor ?? "Não informado",
                    Status = p.Status,
                    Visualizacoes = p.Visualizacoes,
                    Curtidas = p.Curtidas,
                    CriadoEm = p.CriadoEm
                }).ToList();

                var response = new RelatorioResponseDTO<RelatorioPublicacaoDTO>
                {
                    Total = relatorioPublicacoes.Count,
                    Items = relatorioPublicacoes,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    GeradoEm = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("agendamentos")]
        public async Task<ActionResult<RelatorioResponseDTO<RelatorioAgendamentoDTO>>> GetRelatorioAgendamentos(
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            try
            {
                var agendamentos = await _agendamentoService.GetAllAsync();
                var agendamentosList = agendamentos.ToList();

                if (dataInicial.HasValue || dataFinal.HasValue)
                {
                    agendamentosList = agendamentosList.Where(a =>
                    {
                        var dataCriacao = a.CriadoEm.Date;
                        var incluir = true;

                        if (dataInicial.HasValue)
                            incluir = incluir && dataCriacao >= dataInicial.Value.Date;

                        if (dataFinal.HasValue)
                            incluir = incluir && dataCriacao <= dataFinal.Value.Date;

                        return incluir;
                    }).ToList();
                }

                var relatorioAgendamentos = agendamentosList.Select(a => new RelatorioAgendamentoDTO
                {
                    Id = a.Id,
                    Titulo = a.Titulo,
                    Data = a.Data,
                    Usuario = a.Usuario ?? "Não informado",
                    Status = a.Status,
                    CriadoEm = a.CriadoEm
                }).ToList();

                var response = new RelatorioResponseDTO<RelatorioAgendamentoDTO>
                {
                    Total = relatorioAgendamentos.Count,
                    Items = relatorioAgendamentos,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    GeradoEm = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orcamentos")]
        public async Task<ActionResult<RelatorioResponseDTO<RelatorioOrcamentoDTO>>> GetRelatorioOrcamentos(
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            try
            {
                var orcamentos = await _orcamentoService.GetAllAsync();
                var orcamentosList = orcamentos.ToList();

                if (dataInicial.HasValue || dataFinal.HasValue)
                {
                    orcamentosList = orcamentosList.Where(o =>
                    {
                        var dataCriacao = o.CriadoEm.Date;
                        var incluir = true;

                        if (dataInicial.HasValue)
                            incluir = incluir && dataCriacao >= dataInicial.Value.Date;

                        if (dataFinal.HasValue)
                            incluir = incluir && dataCriacao <= dataFinal.Value.Date;

                        return incluir;
                    }).ToList();
                }

                var relatorioOrcamentos = orcamentosList.Select(o => new RelatorioOrcamentoDTO
                {
                    Id = o.Id,
                    Titulo = o.Titulo,
                    Cliente = o.Cliente ?? "Não informado",
                    Valor = o.Valor.HasValue ? o.Valor.Value : 0,
                    Status = o.Status,
                    CriadoEm = o.CriadoEm
                }).ToList();

                var response = new RelatorioResponseDTO<RelatorioOrcamentoDTO>
                {
                    Total = relatorioOrcamentos.Count,
                    Items = relatorioOrcamentos,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    GeradoEm = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("teste")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {

                var usuarios = await _userService.GetAllUsuariosAsync();
                var publicacoes = await _publicacaoService.GetAllAsync();
                var agendamentos = await _agendamentoService.GetAllAsync();
                var orcamentos = await _orcamentoService.GetAllAsync();

                var resultado = new
                {
                    status = "success",
                    timestamp = DateTime.Now,
                    dados = new
                    {
                        totalUsuarios = usuarios.Count(),
                        totalPublicacoes = publicacoes.Count(),
                        totalAgendamentos = agendamentos.Count(),
                        totalOrcamentos = orcamentos.Count()
                    }
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}
