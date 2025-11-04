using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InovalabAPI.Services;
using InovalabAPI.Models.DTOs;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;

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

                return Unauthorized(new { message = "Token inválido ou ausente" });
            }

            // Primeiro tenta buscar como usuário pessoa física
            var usuario = await _userService.GetUsuarioByEmailAsync(email);

            if (usuario != null)
            {

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
                var errors = new Dictionary<string, string[]>();
                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Count > 0)
                    {
                        errors[error.Key] = error.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                    }
                }
                
                return BadRequest(new { 
                    message = "Erro de validação nos dados enviados",
                    errors = errors
                });
            }

            // Extrai o email do token JWT (único e seguro)
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {

                return Unauthorized(new { message = "Token inválido ou ausente" });
            }

            // Busca APENAS o usuário autenticado pelo email do token
            var usuario = await _userService.GetUsuarioByEmailAsync(email);

            if (usuario == null)
            {

                return NotFound(new { message = "Usuário não encontrado" });
            }

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
