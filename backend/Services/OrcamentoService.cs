using Microsoft.EntityFrameworkCore;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly ApplicationDbContext _context;

        public OrcamentoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrcamentoDTO>> GetAllAsync()
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.UsuarioCriador)
                .OrderByDescending(o => o.CriadoEm)
                .ToListAsync();

            return orcamentos.Select(o => MapToDTO(o));
        }

        public async Task<OrcamentoDTO?> GetByIdAsync(int id)
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.UsuarioCriador)
                .FirstOrDefaultAsync(o => o.Id == id);

            return orcamento != null ? MapToDTO(orcamento) : null;
        }

        public async Task<OrcamentoDTO> CreateAsync(CriarOrcamentoDTO criarOrcamentoDto, int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            var orcamento = new Orcamento
            {
                Titulo = criarOrcamentoDto.Titulo,
                Descricao = criarOrcamentoDto.Descricao,
                PrazoEntrega = criarOrcamentoDto.PrazoEntrega,
                PrazoOrcamento = criarOrcamentoDto.PrazoOrcamento,
                Valor = criarOrcamentoDto.Valor,
                Cliente = criarOrcamentoDto.Cliente,
                Responsavel = criarOrcamentoDto.Responsavel ?? $"{usuario.Nome} {usuario.Sobrenome}",
                Status = "pendente",
                UsuarioId = usuarioId,
                CriadoEm = DateTime.UtcNow
            };

            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            orcamento.UsuarioCriador = usuario;
            return MapToDTO(orcamento);
        }

        public async Task<bool> UpdateAsync(int id, AtualizarOrcamentoDTO atualizarOrcamentoDto)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
                return false;

            orcamento.Titulo = atualizarOrcamentoDto.Titulo;
            orcamento.Descricao = atualizarOrcamentoDto.Descricao;
            orcamento.PrazoEntrega = atualizarOrcamentoDto.PrazoEntrega;
            orcamento.PrazoOrcamento = atualizarOrcamentoDto.PrazoOrcamento;
            orcamento.Valor = atualizarOrcamentoDto.Valor;
            orcamento.Cliente = atualizarOrcamentoDto.Cliente;
            orcamento.Responsavel = atualizarOrcamentoDto.Responsavel ?? orcamento.Responsavel;
            orcamento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
                return false;

            _context.Orcamentos.Remove(orcamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarStatusAsync(int id, AlterarStatusOrcamentoDTO alterarStatusDto)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
                return false;

            orcamento.Status = alterarStatusDto.Status.ToLower();
            orcamento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrcamentoDTO>> GetByStatusAsync(string status)
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.UsuarioCriador)
                .Where(o => o.Status == status.ToLower())
                .OrderByDescending(o => o.CriadoEm)
                .ToListAsync();

            return orcamentos.Select(o => MapToDTO(o));
        }

        public async Task<IEnumerable<OrcamentoDTO>> GetByUsuarioAsync(int usuarioId)
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.UsuarioCriador)
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.CriadoEm)
                .ToListAsync();

            return orcamentos.Select(o => MapToDTO(o));
        }

        public async Task<EstatisticasOrcamentoDTO> GetEstatisticasAsync()
        {
            var orcamentos = await _context.Orcamentos.ToListAsync();

            var estatisticas = new EstatisticasOrcamentoDTO
            {
                Total = orcamentos.Count,
                Pendentes = orcamentos.Count(o => o.Status == "pendente"),
                Aprovados = orcamentos.Count(o => o.Status == "aprovado"),
                Rejeitados = orcamentos.Count(o => o.Status == "rejeitado"),
                Concluidos = orcamentos.Count(o => o.Status == "concluido"),
                ValorTotal = orcamentos.Where(o => o.Valor.HasValue && (o.Status == "aprovado" || o.Status == "concluido"))
                                     .Sum(o => o.Valor ?? 0),
                OrcamentosVencendo = (await GetOrcamentosVencendoAsync()).ToList()
            };

            estatisticas.ValorMedio = estatisticas.Total > 0 
                ? orcamentos.Where(o => o.Valor.HasValue).Average(o => o.Valor ?? 0)
                : 0;

            return estatisticas;
        }

        public async Task<EstatisticasOrcamentoDTO> GetEstatisticasPorUsuarioAsync(int usuarioId)
        {
            var orcamentos = await _context.Orcamentos
                .Where(o => o.UsuarioId == usuarioId)
                .ToListAsync();

            var estatisticas = new EstatisticasOrcamentoDTO
            {
                Total = orcamentos.Count,
                Pendentes = orcamentos.Count(o => o.Status == "pendente"),
                Aprovados = orcamentos.Count(o => o.Status == "aprovado"),
                Rejeitados = orcamentos.Count(o => o.Status == "rejeitado"),
                Concluidos = orcamentos.Count(o => o.Status == "concluido"),
                ValorTotal = orcamentos.Where(o => o.Valor.HasValue && (o.Status == "aprovado" || o.Status == "concluido"))
                                     .Sum(o => o.Valor ?? 0),
                OrcamentosVencendo = (await GetOrcamentosVencendoPorUsuarioAsync(usuarioId)).ToList()
            };

            estatisticas.ValorMedio = estatisticas.Total > 0 
                ? orcamentos.Where(o => o.Valor.HasValue).Average(o => o.Valor ?? 0)
                : 0;

            return estatisticas;
        }

        public async Task<IEnumerable<OrcamentoResumoDTO>> GetOrcamentosVencendoPorUsuarioAsync(int usuarioId, int diasProximos = 7)
        {
            var dataLimite = DateTime.UtcNow.AddDays(diasProximos);

            var orcamentos = await _context.Orcamentos
                .Where(o => o.UsuarioId == usuarioId && o.Status == "pendente" && o.PrazoOrcamento <= dataLimite)
                .OrderBy(o => o.PrazoOrcamento)
                .ToListAsync();

            return orcamentos.Select(o => new OrcamentoResumoDTO
            {
                Id = o.Id,
                Titulo = o.Titulo,
                Cliente = o.Cliente,
                PrazoOrcamento = o.PrazoOrcamento,
                PrazoEntrega = o.PrazoEntrega,
                Status = o.Status,
                DiasRestantes = (int)(o.PrazoOrcamento - DateTime.UtcNow).TotalDays,
                PrazoVencido = o.PrazoOrcamento < DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<OrcamentoResumoDTO>> GetOrcamentosVencendoAsync(int diasProximos = 7)
        {
            var dataLimite = DateTime.UtcNow.AddDays(diasProximos);

            var orcamentos = await _context.Orcamentos
                .Where(o => o.Status == "pendente" && o.PrazoOrcamento <= dataLimite)
                .OrderBy(o => o.PrazoOrcamento)
                .ToListAsync();

            return orcamentos.Select(o => new OrcamentoResumoDTO
            {
                Id = o.Id,
                Titulo = o.Titulo,
                Cliente = o.Cliente,
                PrazoOrcamento = o.PrazoOrcamento,
                PrazoEntrega = o.PrazoEntrega,
                Status = o.Status,
                DiasRestantes = (int)(o.PrazoOrcamento - DateTime.UtcNow).TotalDays,
                PrazoVencido = o.PrazoOrcamento < DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<OrcamentoDTO>> GetByFiltroAsync(FiltroOrcamentoDTO filtro)
        {
            var query = _context.Orcamentos.Include(o => o.UsuarioCriador).AsQueryable();

            if (filtro.UsuarioId.HasValue)
                query = query.Where(o => o.UsuarioId == filtro.UsuarioId.Value);

            if (!string.IsNullOrEmpty(filtro.Status))
                query = query.Where(o => o.Status == filtro.Status.ToLower());

            if (!string.IsNullOrEmpty(filtro.Cliente))
                query = query.Where(o => o.Cliente != null && o.Cliente.Contains(filtro.Cliente));

            if (!string.IsNullOrEmpty(filtro.Responsavel))
                query = query.Where(o => o.Responsavel != null && o.Responsavel.Contains(filtro.Responsavel));

            if (filtro.DataInicio.HasValue)
                query = query.Where(o => o.CriadoEm >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(o => o.CriadoEm <= filtro.DataFim.Value);

            if (filtro.ValorMinimo.HasValue)
                query = query.Where(o => o.Valor >= filtro.ValorMinimo.Value);

            if (filtro.ValorMaximo.HasValue)
                query = query.Where(o => o.Valor <= filtro.ValorMaximo.Value);

            if (filtro.PrazosVencidos == true)
                query = query.Where(o => o.PrazoOrcamento < DateTime.UtcNow && o.Status == "pendente");

            if (filtro.DiasPrazo.HasValue)
            {
                var dataLimite = DateTime.UtcNow.AddDays(filtro.DiasPrazo.Value);
                query = query.Where(o => o.PrazoOrcamento <= dataLimite && o.Status == "pendente");
            }

            var orcamentos = await query.OrderByDescending(o => o.CriadoEm).ToListAsync();
            return orcamentos.Select(o => MapToDTO(o));
        }

        private static OrcamentoDTO MapToDTO(Orcamento orcamento)
        {
            return new OrcamentoDTO
            {
                Id = orcamento.Id,
                Titulo = orcamento.Titulo,
                Descricao = orcamento.Descricao,
                PrazoEntrega = orcamento.PrazoEntrega,
                PrazoOrcamento = orcamento.PrazoOrcamento,
                Valor = orcamento.Valor,
                Cliente = orcamento.Cliente,
                Responsavel = orcamento.Responsavel,
                Status = orcamento.Status,
                CriadoEm = orcamento.CriadoEm,
                AtualizadoEm = orcamento.AtualizadoEm
            };
        }
    }
}
