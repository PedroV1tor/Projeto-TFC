using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace InovalabAPI.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public SmtpEmailService(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task EnviarAsync(string destinatarioEmail, string assunto, string corpoHtml)
        {
            var section = _configuration.GetSection("EmailSettings");
            var host = section["Host"];
            var portStr = section["Port"];
            var enableSslStr = section["EnableSsl"];
            var user = section["User"];
            var pass = section["Password"];
            var from = section["From"] ?? user;

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException("EmailSettings.Host não configurado.");
            }

            if (string.IsNullOrWhiteSpace(user))
            {
                throw new InvalidOperationException("EmailSettings.User não configurado.");
            }

            if (string.IsNullOrWhiteSpace(pass))
            {
                throw new InvalidOperationException("EmailSettings.Password não configurado.");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException("EmailSettings.From não configurado.");
            }

            if (!int.TryParse(portStr, out var port))
            {
                port = 587;
            }

            if (!bool.TryParse(enableSslStr, out var enableSsl))
            {
                enableSsl = true;
            }

            try
            {
                using var smtp = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(user, pass),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 30000 // 30 segundos
                };

                using var mail = new MailMessage()
                {
                    From = new MailAddress(from, "Inovalab - Sistema"),
                    Subject = assunto,
                    Body = corpoHtml,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };
                mail.To.Add(destinatarioEmail);

                Console.WriteLine($"=== ENVIANDO EMAIL ===");
                Console.WriteLine($"De: {from}");
                Console.WriteLine($"Para: {destinatarioEmail}");
                Console.WriteLine($"Assunto: {assunto}");
                Console.WriteLine($"=====================");

                await smtp.SendMailAsync(mail);

                Console.WriteLine($"✅ Email enviado com sucesso para {destinatarioEmail}");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Falha no envio de email para {destinatarioEmail}: {ex.Message}";
                Console.WriteLine($"❌ Erro SMTP: {errorMessage}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Em desenvolvimento, fazer fallback (simular envio)
                if (_environment.IsDevelopment())
                {
                    Console.WriteLine($"🔄 AMBIENTE DE DESENVOLVIMENTO - MODO FALLBACK ATIVADO");
                    Console.WriteLine($"📧 Para: {destinatarioEmail}");
                    Console.WriteLine($"📧 Assunto: {assunto}");
                    Console.WriteLine($"📧 === CONTEÚDO DO EMAIL (MODO FALLBACK) ===");
                    Console.WriteLine(corpoHtml);
                    Console.WriteLine($"📧 =========================================");
                    Console.WriteLine($"✅ Email simulado com sucesso (dev mode)");
                    return; // Retorna sem erro apenas em desenvolvimento
                }

                // Em produção, SEMPRE lançar exceção para que o erro seja tratado
                Console.WriteLine($"❌ ERRO EM PRODUÇÃO - Falha no envio de email não pode ser ignorada!");
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }
}
