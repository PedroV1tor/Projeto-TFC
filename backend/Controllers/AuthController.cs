using Microsoft.AspNetCore.Mvc;
using InovalabAPI.Models.DTOs;
using InovalabAPI.Services;

namespace InovalabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);
            
            if (result == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            return Ok(result);
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastro([FromBody] CadastroRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.CadastroAsync(request);
            
            if (!success)
            {
                return BadRequest(new { message = "Email ou nome de usuário já existe" });
            }

            return Ok(new { message = "Usuário cadastrado com sucesso" });
        }

        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.SolicitarRecuperacaoSenhaAsync(request.Email);
            
            if (!success)
            {
                return NotFound(new { message = "Email não encontrado" });
            }

            return Ok(new { message = "Código de verificação enviado para o email" });
        }

        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerificarCodigoRequest request)
        {
            Console.WriteLine($"🌐 API VerificarCodigo chamada para: {request?.Email}");
            Console.WriteLine($"🌐 Código recebido: '{request?.Codigo}'");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine($"❌ ModelState inválido:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"   {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return BadRequest(ModelState);
            }

            var success = await _authService.VerificarCodigoAsync(request);
            
            if (!success)
            {
                Console.WriteLine($"❌ Verificação falhou para: {request.Email}");
                return BadRequest(new { message = "Código inválido ou expirado" });
            }

            Console.WriteLine($"✅ Verificação bem-sucedida para: {request.Email}");
            return Ok(new { message = "Código verificado com sucesso" });
        }

        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.RedefinirSenhaAsync(request);
            
            if (!success)
            {
                return BadRequest(new { message = "Erro ao redefinir senha. Verifique o código." });
            }

            return Ok(new { message = "Senha redefinida com sucesso" });
        }


        [HttpGet("debug-codigos")]
        public IActionResult DebugCodigos()
        {
            try
            {

                var field = typeof(Services.AuthService).GetField("_codigosRecuperacao", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (field?.GetValue(null) is IDictionary<string, (string codigo, DateTime expiracao)> codigos)
                {
                    var resultado = codigos.Select(kvp => new 
                    { 
                        email = kvp.Key, 
                        codigo = kvp.Value.codigo, 
                        expiracao = kvp.Value.expiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                        expirado = DateTime.UtcNow > kvp.Value.expiracao,
                        agora = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList();
                    
                    return Ok(new { 
                        codigos = resultado, 
                        total = resultado.Count,
                        agoraUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
                
                return Ok(new { 
                    codigos = new object[0], 
                    total = 0, 
                    erro = "Campo não encontrado",
                    agoraUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    erro = ex.Message,
                    agoraUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }
    }
}
