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
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var usuario = await _userService.GetUsuarioByEmailAsync(email);
            
            if (usuario == null)
            {
                return NotFound();
            }


            var perfil = new
            {
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

            return Ok(perfil);
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

            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var usuario = await _userService.GetUsuarioByEmailAsync(email);
            
            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }


            usuario.Nome = request.Nome;
            usuario.Sobrenome = request.Sobrenome;
            usuario.Email = request.Email;
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
