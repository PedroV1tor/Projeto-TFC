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
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;

        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
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
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetAll()
        {
            try
            {
                var orcamentos = await _orcamentoService.GetAllAsync();
                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrcamentoDTO>> GetById(int id)
        {
            try
            {
                var orcamento = await _orcamentoService.GetByIdAsync(id);
                if (orcamento == null)
                    return NotFound(new { message = "Orçamento não encontrado" });

                return Ok(orcamento);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetByStatus(string status)
        {
            try
            {
                var orcamentos = await _orcamentoService.GetByStatusAsync(status);
                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var orcamentos = await _orcamentoService.GetByUsuarioAsync(usuarioId);
                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetMeus()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var orcamentos = await _orcamentoService.GetByUsuarioAsync(usuarioId);
                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("estatisticas")]
        public async Task<ActionResult<EstatisticasOrcamentoDTO>> GetEstatisticas()
        {
            try
            {
                // Retorna estatísticas apenas do usuário logado
                var usuarioId = GetCurrentUserId();
                var estatisticas = await _orcamentoService.GetEstatisticasPorUsuarioAsync(usuarioId);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vencendo")]
        public async Task<ActionResult<IEnumerable<OrcamentoResumoDTO>>> GetOrcamentosVencendo([FromQuery] int dias = 7)
        {
            try
            {
                // Retorna orçamentos vencendo apenas do usuário logado
                var usuarioId = GetCurrentUserId();
                var orcamentos = await _orcamentoService.GetOrcamentosVencendoPorUsuarioAsync(usuarioId, dias);
                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrcamentoDTO>> Create([FromBody] CriarOrcamentoDTO criarOrcamentoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = GetCurrentUserId();
                var orcamento = await _orcamentoService.CreateAsync(criarOrcamentoDto, usuarioId);

                return CreatedAtAction(nameof(GetById), new { id = orcamento.Id }, orcamento);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AtualizarOrcamentoDTO atualizarOrcamentoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sucesso = await _orcamentoService.UpdateAsync(id, atualizarOrcamentoDto);
                if (!sucesso)
                    return NotFound(new { message = "Orçamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> AlterarStatus(int id, [FromBody] AlterarStatusOrcamentoDTO alterarStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sucesso = await _orcamentoService.AlterarStatusAsync(id, alterarStatusDto);
                if (!sucesso)
                    return NotFound(new { message = "Orçamento não encontrado" });

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
                var sucesso = await _orcamentoService.DeleteAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Orçamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetByFiltro([FromBody] FiltroOrcamentoDTO filtro)
        {
            try
            {
                // Força o filtro a incluir apenas orçamentos do usuário logado
                var usuarioId = GetCurrentUserId();
                filtro.UsuarioId = usuarioId;

                var orcamentos = await _orcamentoService.GetByFiltroAsync(filtro);
                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/aprovar")]
        public async Task<ActionResult> Aprovar(int id)
        {
            try
            {
                var alterarStatusDto = new AlterarStatusOrcamentoDTO { Status = "aprovado" };
                var sucesso = await _orcamentoService.AlterarStatusAsync(id, alterarStatusDto);
                if (!sucesso)
                    return NotFound(new { message = "Orçamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/rejeitar")]
        public async Task<ActionResult> Rejeitar(int id)
        {
            try
            {
                var alterarStatusDto = new AlterarStatusOrcamentoDTO { Status = "rejeitado" };
                var sucesso = await _orcamentoService.AlterarStatusAsync(id, alterarStatusDto);
                if (!sucesso)
                    return NotFound(new { message = "Orçamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/concluir")]
        public async Task<ActionResult> Concluir(int id)
        {
            try
            {
                var alterarStatusDto = new AlterarStatusOrcamentoDTO { Status = "concluido" };
                var sucesso = await _orcamentoService.AlterarStatusAsync(id, alterarStatusDto);
                if (!sucesso)
                    return NotFound(new { message = "Orçamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
