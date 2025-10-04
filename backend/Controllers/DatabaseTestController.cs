using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InovalabAPI.Services;
using InovalabAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace InovalabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DatabaseTestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IPublicacaoService _publicacaoService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IOrcamentoService _orcamentoService;

        public DatabaseTestController(
            ApplicationDbContext context,
            IUserService userService,
            IPublicacaoService publicacaoService,
            IAgendamentoService agendamentoService,
            IOrcamentoService orcamentoService)
        {
            _context = context;
            _userService = userService;
            _publicacaoService = publicacaoService;
            _agendamentoService = agendamentoService;
            _orcamentoService = orcamentoService;
        }

        [HttpGet("usuarios-direto")]
        public async Task<ActionResult> GetUsuariosDireto()
        {
            try
            {
                Console.WriteLine("🔍 [DatabaseTest] Buscando usuários DIRETAMENTE do contexto...");
                

                var usuarios = await _context.Usuarios.ToListAsync();
                
                Console.WriteLine($"📊 Total de usuários encontrados no banco (direto): {usuarios.Count}");
                
                var usuariosSimplificados = usuarios.Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Sobrenome,
                    u.Email,
                    DataCriacao = u.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    UltimoLogin = u.UltimoLogin?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Nunca",
                    u.Telefone
                }).ToList();

                Console.WriteLine("📋 Primeiros 5 usuários encontrados:");
                foreach (var u in usuariosSimplificados.Take(5))
                {
                    Console.WriteLine($"   - ID: {u.Id}, Nome: {u.Nome} {u.Sobrenome}, Email: {u.Email}, Criado: {u.DataCriacao}");
                }

                return Ok(new
                {
                    fonte = "Entity Framework Context (Direto)",
                    total = usuarios.Count,
                    usuarios = usuariosSimplificados
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DatabaseTest] Erro ao buscar usuários direto: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("usuarios-servico")]
        public async Task<ActionResult> GetUsuariosServico()
        {
            try
            {
                Console.WriteLine("🔍 [DatabaseTest] Buscando usuários através do SERVIÇO...");
                

                var usuarios = await _userService.GetAllUsuariosAsync();
                var usuariosList = usuarios.ToList();
                
                Console.WriteLine($"📊 Total de usuários encontrados no banco (serviço): {usuariosList.Count}");
                
                var usuariosSimplificados = usuariosList.Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Sobrenome,
                    u.Email,
                    DataCriacao = u.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    UltimoLogin = u.UltimoLogin?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Nunca",
                    u.Telefone
                }).ToList();

                Console.WriteLine("📋 Primeiros 5 usuários encontrados:");
                foreach (var u in usuariosSimplificados.Take(5))
                {
                    Console.WriteLine($"   - ID: {u.Id}, Nome: {u.Nome} {u.Sobrenome}, Email: {u.Email}, Criado: {u.DataCriacao}");
                }

                return Ok(new
                {
                    fonte = "UserService",
                    total = usuariosList.Count,
                    usuarios = usuariosSimplificados
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DatabaseTest] Erro ao buscar usuários via serviço: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("publicacoes-direto")]
        public async Task<ActionResult> GetPublicacoesDireto()
        {
            try
            {
                Console.WriteLine("🔍 [DatabaseTest] Buscando publicações DIRETAMENTE do contexto...");
                
                var publicacoes = await _context.Publicacoes.ToListAsync();
                
                Console.WriteLine($"📊 Total de publicações encontradas no banco (direto): {publicacoes.Count}");
                
                var publicacoesSimplificadas = publicacoes.Select(p => new
                {
                    p.Id,
                    p.Titulo,
                    p.Autor,
                    p.Status,
                    CriadoEm = p.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss"),
                    p.Visualizacoes,
                    p.Curtidas
                }).ToList();

                Console.WriteLine("📋 Primeiras 5 publicações encontradas:");
                foreach (var p in publicacoesSimplificadas.Take(5))
                {
                    Console.WriteLine($"   - ID: {p.Id}, Título: {p.Titulo}, Autor: {p.Autor}, Criado: {p.CriadoEm}");
                }

                return Ok(new
                {
                    fonte = "Entity Framework Context (Direto)",
                    total = publicacoes.Count,
                    publicacoes = publicacoesSimplificadas
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DatabaseTest] Erro ao buscar publicações direto: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("agendamentos-direto")]
        public async Task<ActionResult> GetAgendamentosDireto()
        {
            try
            {
                Console.WriteLine("🔍 [DatabaseTest] Buscando agendamentos DIRETAMENTE do contexto...");
                
                var agendamentos = await _context.Agendamentos.ToListAsync();
                
                Console.WriteLine($"📊 Total de agendamentos encontrados no banco (direto): {agendamentos.Count}");
                
                var agendamentosSimplificados = agendamentos.Select(a => new
                {
                    a.Id,
                    a.Titulo,
                    Data = a.Data.ToString("yyyy-MM-dd HH:mm:ss"),
                    a.Usuario,
                    a.Status,
                    CriadoEm = a.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

                Console.WriteLine("📋 Primeiros 5 agendamentos encontrados:");
                foreach (var a in agendamentosSimplificados.Take(5))
                {
                    Console.WriteLine($"   - ID: {a.Id}, Título: {a.Titulo}, Data: {a.Data}, Criado: {a.CriadoEm}");
                }

                return Ok(new
                {
                    fonte = "Entity Framework Context (Direto)",
                    total = agendamentos.Count,
                    agendamentos = agendamentosSimplificados
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DatabaseTest] Erro ao buscar agendamentos direto: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("orcamentos-direto")]
        public async Task<ActionResult> GetOrcamentosDireto()
        {
            try
            {
                Console.WriteLine("🔍 [DatabaseTest] Buscando orçamentos DIRETAMENTE do contexto...");
                
                var orcamentos = await _context.Orcamentos.ToListAsync();
                
                Console.WriteLine($"📊 Total de orçamentos encontrados no banco (direto): {orcamentos.Count}");
                
                var orcamentosSimplificados = orcamentos.Select(o => new
                {
                    o.Id,
                    o.Titulo,
                    o.Cliente,
                    o.Valor,
                    o.Status,
                    CriadoEm = o.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss"),
                    PrazoEntrega = o.PrazoEntrega.ToString("yyyy-MM-dd"),
                    PrazoOrcamento = o.PrazoOrcamento.ToString("yyyy-MM-dd")
                }).ToList();

                Console.WriteLine("📋 Primeiros 5 orçamentos encontrados:");
                foreach (var o in orcamentosSimplificados.Take(5))
                {
                    Console.WriteLine($"   - ID: {o.Id}, Título: {o.Titulo}, Cliente: {o.Cliente}, Valor: R$ {o.Valor:F2}, Criado: {o.CriadoEm}");
                }

                return Ok(new
                {
                    fonte = "Entity Framework Context (Direto)",
                    total = orcamentos.Count,
                    orcamentos = orcamentosSimplificados
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DatabaseTest] Erro ao buscar orçamentos direto: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("estatisticas-completas")]
        public async Task<ActionResult> GetEstatisticasCompletas()
        {
            try
            {
                Console.WriteLine("🔍 [DatabaseTest] Gerando estatísticas completas...");
                

                var usuariosPorData = await _context.Usuarios
                    .GroupBy(u => u.DataCriacao.Date)
                    .Select(g => new { Data = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Data)
                    .ToListAsync();

                var publicacoesPorData = await _context.Publicacoes
                    .GroupBy(p => p.CriadoEm.Date)
                    .Select(g => new { Data = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Data)
                    .ToListAsync();

                var agendamentosPorData = await _context.Agendamentos
                    .GroupBy(a => a.CriadoEm.Date)
                    .Select(g => new { Data = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Data)
                    .ToListAsync();

                var orcamentosPorData = await _context.Orcamentos
                    .GroupBy(o => o.CriadoEm.Date)
                    .Select(g => new { Data = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Data)
                    .ToListAsync();


                var dataMinUsuario = await _context.Usuarios.MinAsync(u => u.DataCriacao);
                var dataMaxUsuario = await _context.Usuarios.MaxAsync(u => u.DataCriacao);

                var resultado = new
                {
                    resumo = new
                    {
                        totalUsuarios = await _context.Usuarios.CountAsync(),
                        totalPublicacoes = await _context.Publicacoes.CountAsync(),
                        totalAgendamentos = await _context.Agendamentos.CountAsync(),
                        totalOrcamentos = await _context.Orcamentos.CountAsync()
                    },
                    rangesDatas = new
                    {
                        usuarios = new
                        {
                            dataMinima = dataMinUsuario.ToString("yyyy-MM-dd HH:mm:ss"),
                            dataMaxima = dataMaxUsuario.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    },
                    distribuicaoPorData = new
                    {
                        usuarios = usuariosPorData.Select(x => new { data = x.Data.ToString("yyyy-MM-dd"), quantidade = x.Count }),
                        publicacoes = publicacoesPorData.Select(x => new { data = x.Data.ToString("yyyy-MM-dd"), quantidade = x.Count }),
                        agendamentos = agendamentosPorData.Select(x => new { data = x.Data.ToString("yyyy-MM-dd"), quantidade = x.Count }),
                        orcamentos = orcamentosPorData.Select(x => new { data = x.Data.ToString("yyyy-MM-dd"), quantidade = x.Count })
                    }
                };

                Console.WriteLine("📊 Estatísticas geradas:");
                Console.WriteLine($"   - Total de usuários: {resultado.resumo.totalUsuarios}");
                Console.WriteLine($"   - Total de publicações: {resultado.resumo.totalPublicacoes}");
                Console.WriteLine($"   - Total de agendamentos: {resultado.resumo.totalAgendamentos}");
                Console.WriteLine($"   - Total de orçamentos: {resultado.resumo.totalOrcamentos}");
                Console.WriteLine($"   - Range de datas dos usuários: {resultado.rangesDatas.usuarios.dataMinima} até {resultado.rangesDatas.usuarios.dataMaxima}");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DatabaseTest] Erro ao gerar estatísticas: {ex.Message}");
                return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}
