using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public interface IAgendamentoService
    {
        Task<IEnumerable<AgendamentoDTO>> GetAllAsync();
        Task<AgendamentoDTO?> GetByIdAsync(int id);
        Task<AgendamentoDTO> CreateAsync(CriarAgendamentoDTO criarAgendamentoDto, int usuarioId);
        Task<bool> UpdateAsync(int id, AtualizarAgendamentoDTO atualizarAgendamentoDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AlterarStatusAsync(int id, AlterarStatusAgendamentoDTO alterarStatusDto);
        Task<IEnumerable<AgendamentoDTO>> GetByStatusAsync(string status);
        Task<IEnumerable<AgendamentoDTO>> GetByUsuarioAsync(int usuarioId);
        Task<IEnumerable<AgendamentoDTO>> GetProximosEventosAsync(int diasProximos = 7);
        Task<IEnumerable<AgendamentoDTO>> GetProximosEventosPorUsuarioAsync(int usuarioId, int diasProximos = 7);
        Task<IEnumerable<AgendamentoDTO>> GetByFiltroAsync(FiltroAgendamentoDTO filtro);
    }
}
