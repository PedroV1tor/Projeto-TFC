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
                Console.WriteLine($"🔍 [RelatorioController] Gerando resumo geral");
                Console.WriteLine($"📅 Parâmetros: dataInicial={dataInicial}, dataFinal={dataFinal}");
                

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
                    UsuariosAtivos = usuariosFiltrados.Count(), // Todos os usuários que acessam são considerados ativos
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

                Console.WriteLine($"✅ [RelatorioController] Resumo gerado: {resumo.TotalUsuarios} usuários, {resumo.TotalPublicacoes} publicações");
                
                return Ok(resumo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [RelatorioController] Erro ao gerar resumo: {ex.Message}");
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
                Console.WriteLine($"🔍 [RelatorioController] Iniciando busca de usuários");
                Console.WriteLine($"📅 Parâmetros: dataInicial={dataInicial}, dataFinal={dataFinal}");
                
                var usuarios = await _userService.GetAllUsuariosAsync();
                var usuariosList = usuarios.ToList();
                
                Console.WriteLine($"📊 Total de usuários encontrados no banco: {usuariosList.Count}");
                
                if (usuariosList.Any())
                {
                    Console.WriteLine($"📋 Primeiros usuários:");
                    foreach (var u in usuariosList.Take(3))
                    {
                        Console.WriteLine($"   - {u.Nome} {u.Sobrenome} (Criado em: {u.DataCriacao:yyyy-MM-dd})");
                    }
                }
                

                if (dataInicial.HasValue || dataFinal.HasValue)
                {
                    Console.WriteLine($"🔄 Aplicando filtros de data...");
                    var usuariosAntesDoFiltro = usuariosList.Count;
                    
                    usuariosList = usuariosList.Where(u => 
                    {
                        var dataCriacao = u.DataCriacao.Date;
                        var incluir = true;
                        
                        Console.WriteLine($"🗓️ Verificando usuário {u.Nome}: dataCriacao={dataCriacao:yyyy-MM-dd}, dataInicial={dataInicial:yyyy-MM-dd}, dataFinal={dataFinal:yyyy-MM-dd}");
                        
                        if (dataInicial.HasValue)
                        {
                            incluir = incluir && dataCriacao >= dataInicial.Value.Date;
                            Console.WriteLine($"   ➡️ Filtro dataInicial: {dataCriacao >= dataInicial.Value.Date} (incluir={incluir})");
                        }
                            
                        if (dataFinal.HasValue)
                        {
                            incluir = incluir && dataCriacao <= dataFinal.Value.Date;
                            Console.WriteLine($"   ➡️ Filtro dataFinal: {dataCriacao <= dataFinal.Value.Date} (incluir={incluir})");
                        }
                            
                        Console.WriteLine($"   ✅ Resultado final para {u.Nome}: {incluir}");
                        return incluir;
                    }).ToList();
                    
                    Console.WriteLine($"📈 Usuários após filtro: {usuariosList.Count} (eram {usuariosAntesDoFiltro})");
                }
                else
                {
                    Console.WriteLine($"🔄 Nenhum filtro de data aplicado - retornando todos os usuários");
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

                Console.WriteLine($"✅ [RelatorioController] Retornando resposta com {response.Total} usuários");
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

        [HttpGet("teste")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                Console.WriteLine("🧪 [RelatorioController] Endpoint de teste chamado");
                

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
                
                Console.WriteLine($"✅ [RelatorioController] Teste concluído: {resultado.dados.totalUsuarios} usuários, {resultado.dados.totalPublicacoes} publicações, {resultado.dados.totalAgendamentos} agendamentos, {resultado.dados.totalOrcamentos} orçamentos");
                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [RelatorioController] Erro no teste: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
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
    }
}
