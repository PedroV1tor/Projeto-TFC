using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public interface IOrcamentoService
    {
        Task<IEnumerable<OrcamentoDTO>> GetAllAsync();
        Task<OrcamentoDTO?> GetByIdAsync(int id);
        Task<OrcamentoDTO> CreateAsync(CriarOrcamentoDTO criarOrcamentoDto, int usuarioId);
        Task<bool> UpdateAsync(int id, AtualizarOrcamentoDTO atualizarOrcamentoDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AlterarStatusAsync(int id, AlterarStatusOrcamentoDTO alterarStatusDto);
        Task<IEnumerable<OrcamentoDTO>> GetByStatusAsync(string status);
        Task<IEnumerable<OrcamentoDTO>> GetByUsuarioAsync(int usuarioId);
        Task<EstatisticasOrcamentoDTO> GetEstatisticasAsync();
        Task<IEnumerable<OrcamentoResumoDTO>> GetOrcamentosVencendoAsync(int diasProximos = 7);
        Task<IEnumerable<OrcamentoDTO>> GetByFiltroAsync(FiltroOrcamentoDTO filtro);
    }
}
