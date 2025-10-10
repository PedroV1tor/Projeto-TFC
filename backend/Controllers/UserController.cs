using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InovalabAPI.Services;
using InovalabAPI.Models.DTOs;
using System.Security.Claims;

namespace InovalabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> GetPerfil()
        {
            // Extrai o email do token JWT (único e seguro)
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("⚠️ [Perfil] Tentativa de acesso sem email no token JWT");
                return Unauthorized(new { message = "Token inválido ou ausente" });
            }

            Console.WriteLine($"🔐 [Perfil] Usuário/Empresa autenticado solicitando perfil: {email}");

            // Primeiro tenta buscar como usuário pessoa física
            var usuario = await _userService.GetUsuarioByEmailAsync(email);
            
            if (usuario != null)
            {
                Console.WriteLine($"✅ [Perfil] Retornando dados do usuário ID: {usuario.Id}, Email: {email}");

                // Retorna dados de pessoa física
                var perfilUsuario = new
                {
                    Tipo = "usuario",
                    usuario.Id,
                    usuario.Nome,
                    usuario.Sobrenome,
                    usuario.Email,
                    usuario.Matricula,
                    usuario.NomeUsuario,
                    usuario.Telefone,
                    Endereco = usuario.Endereco != null ? new
                    {
                        usuario.Endereco.CEP,
                        usuario.Endereco.Rua,
                        usuario.Endereco.Bairro,
                        usuario.Endereco.Numero,
                        usuario.Endereco.Referencia,
                        usuario.Endereco.Complemento
                    } : null,
                    usuario.DataCriacao,
                    usuario.UltimoLogin
                };

                return Ok(perfilUsuario);
            }

            // Se não encontrou como pessoa física, busca como empresa
            var empresa = await _userService.GetEmpresaByEmailAsync(email);
            
            if (empresa != null)
            {
                Console.WriteLine($"✅ [Perfil] Retornando dados da empresa ID: {empresa.Id}, Email: {email}");

                // Retorna dados de empresa
                var perfilEmpresa = new
                {
                    Tipo = "empresa",
                    empresa.Id,
                    empresa.RazaoSocial,
                    empresa.NomeFantasia,
                    empresa.CNPJ,
                    empresa.Email,
                    empresa.Telefone,
                    empresa.ResponsavelNome,
                    empresa.ResponsavelTelefone,
                    Endereco = empresa.Endereco != null ? new
                    {
                        empresa.Endereco.CEP,
                        empresa.Endereco.Rua,
                        empresa.Endereco.Bairro,
                        empresa.Endereco.Numero,
                        empresa.Endereco.Referencia,
                        empresa.Endereco.Complemento
                    } : null,
                    empresa.DataCriacao,
                    empresa.UltimoLogin
                };

                return Ok(perfilEmpresa);
            }

            Console.WriteLine($"❌ [Perfil] Nenhum usuário ou empresa encontrado para email: {email}");
            return NotFound(new { message = "Perfil não encontrado" });
        }

        [HttpGet("todos")]
        [Authorize] // Aqui você pode adicionar autorização de admin se necessário
        public async Task<IActionResult> GetTodosUsuarios()
        {
            var usuarios = await _userService.GetAllUsuariosAsync();
            
            var usuariosData = usuarios.Select(u => new
            {
                u.Id,
                u.Nome,
                u.Sobrenome,
                u.Email,
                u.Matricula,
                u.NomeUsuario,
                u.Telefone,
                u.DataCriacao,
                u.UltimoLogin
            });

            return Ok(usuariosData);
        }

        [HttpPut("atualizar")]
        public async Task<IActionResult> AtualizarPerfil([FromBody] AtualizarUsuarioRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Extrai o email do token JWT (único e seguro)
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("⚠️ [Atualizar Perfil] Tentativa de atualização sem email no token JWT");
                return Unauthorized(new { message = "Token inválido ou ausente" });
            }

            Console.WriteLine($"🔐 [Atualizar Perfil] Usuário autenticado solicitando atualização: {email}");

            // Busca APENAS o usuário autenticado pelo email do token
            var usuario = await _userService.GetUsuarioByEmailAsync(email);
            
            if (usuario == null)
            {
                Console.WriteLine($"❌ [Atualizar Perfil] Usuário não encontrado para email: {email}");
                return NotFound(new { message = "Usuário não encontrado" });
            }

            Console.WriteLine($"✅ [Atualizar Perfil] Atualizando dados do usuário ID: {usuario.Id}, Email: {email}");

            // Atualiza APENAS os dados permitidos (email NÃO pode ser alterado)
            usuario.Nome = request.Nome;
            usuario.Sobrenome = request.Sobrenome;
            // usuario.Email NÃO é atualizado por segurança (é usado para autenticação)
            usuario.NomeUsuario = request.NomeUsuario;
            usuario.Telefone = request.Telefone;

            if (usuario.Endereco == null)
            {
                usuario.Endereco = new InovalabAPI.Models.EnderecoUsuario
                {
                    UsuarioId = usuario.Id,
                    CEP = request.Endereco.CEP,
                    Rua = request.Endereco.Rua,
                    Bairro = request.Endereco.Bairro,
                    Numero = request.Endereco.Numero,
                    Referencia = request.Endereco.Referencia,
                    Complemento = request.Endereco.Complemento,
                    DataCriacao = DateTime.UtcNow
                };
            }
            else
            {
                usuario.Endereco.CEP = request.Endereco.CEP;
                usuario.Endereco.Rua = request.Endereco.Rua;
                usuario.Endereco.Bairro = request.Endereco.Bairro;
                usuario.Endereco.Numero = request.Endereco.Numero;
                usuario.Endereco.Referencia = request.Endereco.Referencia;
                usuario.Endereco.Complemento = request.Endereco.Complemento;
            }

            var success = await _userService.UpdateUsuarioAsync(usuario);
            
            if (!success)
            {
                return StatusCode(500, new { message = "Erro interno do servidor ao atualizar usuário" });
            }

            return Ok(new { message = "Perfil atualizado com sucesso" });
        }

        [HttpDelete("{id}")]
        [Authorize] // Aqui você pode adicionar autorização de admin se necessário
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var success = await _userService.DeleteUsuarioAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(new { message = "Usuário removido com sucesso" });
        }
    }
}
