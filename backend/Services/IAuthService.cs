using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> CadastroAsync(CadastroRequest request);
        Task<bool> CadastroEmpresaAsync(CadastroEmpresaRequest request);
        Task<bool> SolicitarRecuperacaoSenhaAsync(string email);
        Task<bool> VerificarCodigoAsync(VerificarCodigoRequest request);
        Task<bool> RedefinirSenhaAsync(RedefinirSenhaRequest request);
        string GenerateJwtToken(int userId, string email, string nome, string nomeUsuario, bool isAdmin = false);
    }
}
