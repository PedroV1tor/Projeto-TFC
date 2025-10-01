using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public interface IPublicacaoService
    {
        Task<IEnumerable<PublicacaoDTO>> GetAllAsync();
        Task<PublicacaoDTO?> GetByIdAsync(int id);
        Task<PublicacaoDTO> CreateAsync(CriarPublicacaoDTO criarPublicacaoDto, int usuarioId);
        Task<bool> UpdateAsync(int id, AtualizarPublicacaoDTO atualizarPublicacaoDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AlterarStatusAsync(int id, AlterarStatusDTO alterarStatusDto);
        Task<IEnumerable<PublicacaoDTO>> GetAtivasAsync();
        Task<IEnumerable<PublicacaoTimelineDTO>> GetTimelineAsync();
        Task<bool> IncrementarVisualizacoesAsync(int id);
        Task<bool> CurtirPublicacaoAsync(int id);
        Task<IEnumerable<PublicacaoDTO>> GetByUsuarioAsync(int usuarioId);
        Task<IEnumerable<PublicacaoDTO>> GetByStatusAsync(string status);
    }
}
