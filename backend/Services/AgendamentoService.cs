using Microsoft.EntityFrameworkCore;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly ApplicationDbContext _context;

        public AgendamentoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgendamentoDTO>> GetAllAsync()
        {
            var agendamentos = await _context.Agendamentos
                .Include(a => a.UsuarioCriador)
                .OrderByDescending(a => a.CriadoEm)
                .ToListAsync();

            return agendamentos.Select(a => MapToDTO(a));
        }

        public async Task<AgendamentoDTO?> GetByIdAsync(int id)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.UsuarioCriador)
                .FirstOrDefaultAsync(a => a.Id == id);

            return agendamento != null ? MapToDTO(agendamento) : null;
        }

        public async Task<AgendamentoDTO> CreateAsync(CriarAgendamentoDTO criarAgendamentoDto, int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            var agendamento = new Agendamento
            {
                Titulo = criarAgendamentoDto.Titulo,
                Data = criarAgendamentoDto.Data,
                Descricao = criarAgendamentoDto.Descricao,
                Usuario = criarAgendamentoDto.Usuario ?? $"{usuario.Nome} {usuario.Sobrenome}",
                Status = "pendente",
                UsuarioId = usuarioId,
                CriadoEm = DateTime.UtcNow
            };

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            agendamento.UsuarioCriador = usuario;
            return MapToDTO(agendamento);
        }

        public async Task<bool> UpdateAsync(int id, AtualizarAgendamentoDTO atualizarAgendamentoDto)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
                return false;

            agendamento.Titulo = atualizarAgendamentoDto.Titulo;
            agendamento.Data = atualizarAgendamentoDto.Data;
            agendamento.Descricao = atualizarAgendamentoDto.Descricao;
            agendamento.Usuario = atualizarAgendamentoDto.Usuario ?? agendamento.Usuario;
            agendamento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
                return false;

            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarStatusAsync(int id, AlterarStatusAgendamentoDTO alterarStatusDto)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
                return false;

            agendamento.Status = alterarStatusDto.Status;
            agendamento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AgendamentoDTO>> GetByStatusAsync(string status)
        {
            var agendamentos = await _context.Agendamentos
                .Include(a => a.UsuarioCriador)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CriadoEm)
                .ToListAsync();

            return agendamentos.Select(a => MapToDTO(a));
        }

        public async Task<IEnumerable<AgendamentoDTO>> GetByUsuarioAsync(int usuarioId)
        {
            var agendamentos = await _context.Agendamentos
                .Include(a => a.UsuarioCriador)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.CriadoEm)
                .ToListAsync();

            return agendamentos.Select(a => MapToDTO(a));
        }

        public async Task<IEnumerable<AgendamentoDTO>> GetProximosEventosAsync(int diasProximos = 7)
        {
            var dataLimite = DateTime.UtcNow.AddDays(diasProximos);
            
            var agendamentos = await _context.Agendamentos
                .Include(a => a.UsuarioCriador)
                .Where(a => a.Status == "ativo" && a.Data >= DateTime.UtcNow && a.Data <= dataLimite)
                .OrderBy(a => a.Data)
                .ToListAsync();

            return agendamentos.Select(a => MapToDTO(a));
        }

        public async Task<IEnumerable<AgendamentoDTO>> GetByFiltroAsync(FiltroAgendamentoDTO filtro)
        {
            var query = _context.Agendamentos.Include(a => a.UsuarioCriador).AsQueryable();

            if (!string.IsNullOrEmpty(filtro.Status))
                query = query.Where(a => a.Status == filtro.Status);

            if (!string.IsNullOrEmpty(filtro.Usuario))
                query = query.Where(a => a.Usuario != null && a.Usuario.Contains(filtro.Usuario));

            if (filtro.DataInicio.HasValue)
                query = query.Where(a => a.Data >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(a => a.Data <= filtro.DataFim.Value);

            if (filtro.ProximosEventos == true && filtro.ProximosDias.HasValue)
            {
                var dataLimite = DateTime.UtcNow.AddDays(filtro.ProximosDias.Value);
                query = query.Where(a => a.Data >= DateTime.UtcNow && a.Data <= dataLimite);
            }

            var agendamentos = await query.OrderByDescending(a => a.CriadoEm).ToListAsync();
            return agendamentos.Select(a => MapToDTO(a));
        }

        private static AgendamentoDTO MapToDTO(Agendamento agendamento)
        {
            return new AgendamentoDTO
            {
                Id = agendamento.Id,
                Titulo = agendamento.Titulo,
                Data = agendamento.Data,
                Descricao = agendamento.Descricao,
                Usuario = agendamento.Usuario,
                Status = agendamento.Status,
                CriadoEm = agendamento.CriadoEm,
                AtualizadoEm = agendamento.AtualizadoEm
            };
        }
    }
}
