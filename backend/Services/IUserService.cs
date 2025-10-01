using InovalabAPI.Models;

namespace InovalabAPI.Services
{
    public interface IUserService
    {
        Task<Usuario?> GetUsuarioByEmailAsync(string email);
        Task<Usuario?> GetUsuarioByIdAsync(int id);
        Task<IEnumerable<Usuario>> GetAllUsuariosAsync();
        Task<bool> UpdateUsuarioAsync(Usuario usuario);
        Task<bool> DeleteUsuarioAsync(int id);
    }
}
