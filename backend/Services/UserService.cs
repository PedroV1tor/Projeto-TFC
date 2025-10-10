using Microsoft.EntityFrameworkCore;
using InovalabAPI.Data;
using InovalabAPI.Models;

namespace InovalabAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            Console.WriteLine($"🔍 [UserService] Buscando usuário com email: {email}");
            
            var usuario = await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
            
            if (usuario != null)
            {
                Console.WriteLine($"✅ [UserService] Usuário encontrado: ID={usuario.Id}, Nome={usuario.Nome} {usuario.Sobrenome}");
                Console.WriteLine($"📧 [UserService] Email confirmado: {usuario.Email}");
                Console.WriteLine($"🏠 [UserService] Endereço presente: {(usuario.Endereco != null ? "Sim" : "Não")}");
            }
            else
            {
                Console.WriteLine($"❌ [UserService] Nenhum usuário encontrado com email: {email}");
            }
            
            return usuario;
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.Id == id && u.Ativo);
        }

        public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Endereco)
                .Where(u => u.Ativo)
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }

        public async Task<bool> UpdateUsuarioAsync(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return false;
            }


            usuario.Ativo = false;
            return await UpdateUsuarioAsync(usuario);
        }

        // Métodos para Empresa
        public async Task<Empresa?> GetEmpresaByEmailAsync(string email)
        {
            Console.WriteLine($"🔍 [UserService] Buscando empresa com email: {email}");
            
            var empresa = await _context.Empresas
                .Include(e => e.Endereco)
                .FirstOrDefaultAsync(e => e.Email == email && e.Ativo);
            
            if (empresa != null)
            {
                Console.WriteLine($"✅ [UserService] Empresa encontrada: ID={empresa.Id}, Razão Social={empresa.RazaoSocial}");
                Console.WriteLine($"📧 [UserService] Email confirmado: {empresa.Email}");
                Console.WriteLine($"🏢 [UserService] CNPJ: {empresa.CNPJ}");
                Console.WriteLine($"🏠 [UserService] Endereço presente: {(empresa.Endereco != null ? "Sim" : "Não")}");
            }
            else
            {
                Console.WriteLine($"❌ [UserService] Nenhuma empresa encontrada com email: {email}");
            }
            
            return empresa;
        }

        public async Task<Empresa?> GetEmpresaByIdAsync(int id)
        {
            return await _context.Empresas
                .Include(e => e.Endereco)
                .FirstOrDefaultAsync(e => e.Id == id && e.Ativo);
        }

        public async Task<bool> UpdateEmpresaAsync(Empresa empresa)
        {
            try
            {
                _context.Empresas.Update(empresa);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
