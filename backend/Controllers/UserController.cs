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

            // Retorna dados do usuário sem a senha
            var perfil = new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Sobrenome,
                usuario.Email,
                usuario.Matricula,
                usuario.NomeUsuario,
                usuario.Telefone,
                usuario.CEP,
                usuario.Rua,
                usuario.Bairro,
                usuario.Numero,
                usuario.Referencia,
                usuario.Complemento,
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

            // Atualizar dados do usuário
            usuario.Nome = request.Nome;
            usuario.Sobrenome = request.Sobrenome;
            usuario.Email = request.Email;
            usuario.NomeUsuario = request.NomeUsuario;
            usuario.Telefone = request.Telefone;
            usuario.CEP = request.CEP;
            usuario.Rua = request.Rua;
            usuario.Bairro = request.Bairro;
            usuario.Numero = request.Numero;
            usuario.Referencia = request.Referencia;
            usuario.Complemento = request.Complemento;

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
