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
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentoController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
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
        public async Task<ActionResult<IEnumerable<AgendamentoDTO>>> GetAll()
        {
            try
            {
                var agendamentos = await _agendamentoService.GetAllAsync();
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AgendamentoDTO>> GetById(int id)
        {
            try
            {
                var agendamento = await _agendamentoService.GetByIdAsync(id);
                if (agendamento == null)
                    return NotFound(new { message = "Agendamento não encontrado" });

                return Ok(agendamento);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AgendamentoDTO>>> GetByStatus(string status)
        {
            try
            {
                var agendamentos = await _agendamentoService.GetByStatusAsync(status);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<AgendamentoDTO>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var agendamentos = await _agendamentoService.GetByUsuarioAsync(usuarioId);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<AgendamentoDTO>>> GetMeus()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var agendamentos = await _agendamentoService.GetByUsuarioAsync(usuarioId);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("proximos")]
        public async Task<ActionResult<IEnumerable<AgendamentoDTO>>> GetProximosEventos([FromQuery] int dias = 7)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var agendamentos = await _agendamentoService.GetProximosEventosPorUsuarioAsync(usuarioId, dias);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDTO>> Create([FromBody] CriarAgendamentoDTO criarAgendamentoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = GetCurrentUserId();
                var agendamento = await _agendamentoService.CreateAsync(criarAgendamentoDto, usuarioId);

                return CreatedAtAction(nameof(GetById), new { id = agendamento.Id }, agendamento);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AtualizarAgendamentoDTO atualizarAgendamentoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sucesso = await _agendamentoService.UpdateAsync(id, atualizarAgendamentoDto);
                if (!sucesso)
                    return NotFound(new { message = "Agendamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> AlterarStatus(int id, [FromBody] AlterarStatusAgendamentoDTO alterarStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sucesso = await _agendamentoService.AlterarStatusAsync(id, alterarStatusDto);
                if (!sucesso)
                    return NotFound(new { message = "Agendamento não encontrado" });

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
                var sucesso = await _agendamentoService.DeleteAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Agendamento não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<AgendamentoDTO>>> GetByFiltro([FromBody] FiltroAgendamentoDTO filtro)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                filtro.UsuarioId = usuarioId;

                var agendamentos = await _agendamentoService.GetByFiltroAsync(filtro);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
