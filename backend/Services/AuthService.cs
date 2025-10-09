using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private static readonly Dictionary<string, (string codigo, DateTime expiracao)> _codigosRecuperacao = new();
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                return null;
            }


            usuario.UltimoLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(usuario.Id, usuario.Email, usuario.Nome, usuario.NomeUsuario);
            
            return new LoginResponse
            {
                Token = token,
                Email = usuario.Email,
                Nome = usuario.Nome,
                NomeUsuario = usuario.NomeUsuario,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<bool> CadastroAsync(CadastroRequest request)
        {

            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return false;
            }


            if (await _context.Usuarios.AnyAsync(u => u.NomeUsuario == request.NomeUsuario))
            {
                return false;
            }

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Sobrenome = request.Sobrenome,
                Email = request.Email,
                Matricula = request.Matricula,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                NomeUsuario = request.NomeUsuario,
                Telefone = request.Telefone,
                DataCriacao = DateTime.UtcNow,
                Ativo = true,
                Endereco = new EnderecoUsuario
                {
                    CEP = request.Endereco.CEP,
                    Rua = request.Endereco.Rua,
                    Bairro = request.Endereco.Bairro,
                    Numero = request.Endereco.Numero,
                    Referencia = request.Endereco.Referencia,
                    Complemento = request.Endereco.Complemento,
                    DataCriacao = DateTime.UtcNow
                }
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SolicitarRecuperacaoSenhaAsync(string email)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

            if (usuario == null)
            {
                return false;
            }


            var codigo = new Random().Next(10000, 99999).ToString();
            var expiracao = DateTime.UtcNow.AddMinutes(15); // Expira em 15 minutos
            
            Console.WriteLine($"✅ Código gerado para {email}: {codigo} (expira em 15min)");
            

            LimparCodigosExpirados();
            

            _codigosRecuperacao[email] = (codigo, expiracao);

            var appName = _configuration["App:Name"] ?? "Inovalab";
            var assunto = $"{appName} - Código de recuperação de senha";
            var corpo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .code {{ font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #667eea; text-align: center; background: white; padding: 20px; border: 2px dashed #667eea; border-radius: 8px; margin: 20px 0; }}
        .info {{ background: #e3f2fd; padding: 15px; border-radius: 5px; border-left: 4px solid #2196f3; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{appName}</h1>
            <p>Recuperação de Senha</p>
        </div>
        <div class='content'>
            <h2>Olá!</h2>
            <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
            
            <p>Use o código abaixo para verificar sua identidade:</p>
            
            <div class='code'>{codigo}</div>
            
            <div class='info'>
                <strong>⏰ Importante:</strong>
                <ul>
                    <li>Este código expira em <strong>15 minutos</strong></li>
                    <li>Use apenas uma vez</li>
                    <li>Não compartilhe este código com ninguém</li>
                </ul>
            </div>
            
            <p>Se você não solicitou esta recuperação de senha, pode ignorar este e-mail com segurança.</p>
        </div>
        <div class='footer'>
            <p>Este é um e-mail automático, não responda.</p>
            <p>&copy; 2025 {appName} - Sistema de Inovação</p>
        </div>
    </div>
</body>
</html>";

            await _emailService.EnviarAsync(email, assunto, corpo);

            return true;
        }

        public async Task<bool> VerificarCodigoAsync(VerificarCodigoRequest request)
        {

            var codigoLimpo = request.Codigo?.Trim();
            
            Console.WriteLine($"🔍 Verificando código para {request.Email}");
            Console.WriteLine($"   Código recebido: '{request.Codigo}' (original)");
            Console.WriteLine($"   Código limpo: '{codigoLimpo}'");
            

            LimparCodigosExpirados();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

            if (usuario == null)
            {
                Console.WriteLine($"❌ Usuário não encontrado: {request.Email}");
                return false;
            }


            if (!_codigosRecuperacao.ContainsKey(request.Email))
            {
                Console.WriteLine($"❌ Nenhum código encontrado para: {request.Email}");
                Console.WriteLine($"   Códigos disponíveis: {string.Join(", ", _codigosRecuperacao.Keys)}");
                return false;
            }

            var (codigoArmazenado, expiracao) = _codigosRecuperacao[request.Email];
            Console.WriteLine($"   Código armazenado: '{codigoArmazenado}'");
            Console.WriteLine($"   Expira em: {expiracao}");
            Console.WriteLine($"   Agora: {DateTime.UtcNow}");


            if (DateTime.UtcNow > expiracao)
            {
                Console.WriteLine($"❌ Código expirado para: {request.Email}");
                _codigosRecuperacao.Remove(request.Email);
                return false;
            }


            var resultado = codigoArmazenado == codigoLimpo;
            Console.WriteLine($"   Comparação: '{codigoArmazenado}' == '{codigoLimpo}' = {resultado}");
            Console.WriteLine($"{(resultado ? "✅" : "❌")} Verificação: {request.Email} - {(resultado ? "SUCESSO" : "FALHOU")}");
            
            return resultado;
        }

        public async Task<bool> RedefinirSenhaAsync(RedefinirSenhaRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

            if (usuario == null)
            {
                return false;
            }


            LimparCodigosExpirados();


            if (!_codigosRecuperacao.ContainsKey(request.Email))
            {
                return false;
            }

            var (codigoArmazenado, expiracao) = _codigosRecuperacao[request.Email];
            

            if (DateTime.UtcNow > expiracao || codigoArmazenado != request.Codigo)
            {
                return false;
            }


            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            await _context.SaveChangesAsync();


            _codigosRecuperacao.Remove(request.Email);

            return true;
        }

        private void LimparCodigosExpirados()
        {
            var agora = DateTime.UtcNow;
            var chavesExpiradas = _codigosRecuperacao
                .Where(kvp => agora > kvp.Value.expiracao)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var chave in chavesExpiradas)
            {
                _codigosRecuperacao.Remove(chave);
                Console.WriteLine($"Código expirado removido para: {chave}");
            }
        }

        public string GenerateJwtToken(int userId, string email, string nome, string nomeUsuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "MinhaChaveSecretaSuperSeguraComMaisDe32Caracteres123456";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Email, email),
                new Claim("nome", nome),
                new Claim("nomeUsuario", nomeUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryInHours"] ?? "24")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
