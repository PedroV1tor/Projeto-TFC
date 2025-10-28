using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InovalabAPI.Models.DTOs;
using InovalabAPI.Services;

namespace InovalabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PublicacaoController : ControllerBase
    {
        private readonly IPublicacaoService _publicacaoService;

        public PublicacaoController(IPublicacaoService publicacaoService)
        {
            _publicacaoService = publicacaoService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Token JWT inválido ou usuário não autenticado");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new ArgumentException($"O valor do claim NameIdentifier '{userIdClaim}' não é um ID de usuário válido");
            }

            return userId;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublicacaoDTO>>> GetAll()
        {
            try
            {
                var publicacoes = await _publicacaoService.GetAllAsync();
                return Ok(publicacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicacaoDTO>> GetById(int id)
        {
            try
            {
                var publicacao = await _publicacaoService.GetByIdAsync(id);
                if (publicacao == null)
                    return NotFound(new { message = "Publicação não encontrada" });


                await _publicacaoService.IncrementarVisualizacoesAsync(id);

                return Ok(publicacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ativas")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublicacaoDTO>>> GetAtivas()
        {
            try
            {
                var publicacoes = await _publicacaoService.GetAtivasAsync();
                return Ok(publicacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("timeline")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublicacaoTimelineDTO>>> GetTimeline()
        {
            try
            {
                var timeline = await _publicacaoService.GetTimelineAsync();
                return Ok(timeline);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<PublicacaoDTO>>> GetByStatus(string status)
        {
            try
            {
                var publicacoes = await _publicacaoService.GetByStatusAsync(status);
                return Ok(publicacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<PublicacaoDTO>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var publicacoes = await _publicacaoService.GetByUsuarioAsync(usuarioId);
                return Ok(publicacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("minhas")]
        public async Task<ActionResult<IEnumerable<PublicacaoDTO>>> GetMinhas()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var publicacoes = await _publicacaoService.GetByUsuarioAsync(usuarioId);
                return Ok(publicacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PublicacaoDTO>> Create([FromBody] CriarPublicacaoDTO criarPublicacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = GetCurrentUserId();
                var publicacao = await _publicacaoService.CreateAsync(criarPublicacaoDto, usuarioId);

                return CreatedAtAction(nameof(GetById), new { id = publicacao.Id }, publicacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AtualizarPublicacaoDTO atualizarPublicacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sucesso = await _publicacaoService.UpdateAsync(id, atualizarPublicacaoDto);
                if (!sucesso)
                    return NotFound(new { message = "Publicação não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> AlterarStatus(int id, [FromBody] AlterarStatusDTO alterarStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sucesso = await _publicacaoService.AlterarStatusAsync(id, alterarStatusDto);
                if (!sucesso)
                    return NotFound(new { message = "Publicação não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/curtir")]
        public async Task<ActionResult> CurtirPublicacao(int id)
        {
            try
            {
                var sucesso = await _publicacaoService.CurtirPublicacaoAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Publicação não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var sucesso = await _publicacaoService.DeleteAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Publicação não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
