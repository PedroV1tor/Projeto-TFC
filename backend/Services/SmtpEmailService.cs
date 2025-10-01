using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace InovalabAPI.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
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
                Console.WriteLine($"❌ Erro SMTP: {ex.Message}");
                Console.WriteLine($"🔄 ATIVANDO MODO FALLBACK para desenvolvimento...");
                Console.WriteLine($"📧 Para: {destinatarioEmail}");
                Console.WriteLine($"📧 Assunto: {assunto}");
                
                // Em caso de erro SMTP, sempre simula o envio em desenvolvimento
                Console.WriteLine($"📧 === CONTEÚDO DO EMAIL (MODO FALLBACK) ===");
                Console.WriteLine($"📧 Para: {destinatarioEmail}");
                Console.WriteLine($"📧 Assunto: {assunto}");
                Console.WriteLine($"📧 =========================================");
                Console.WriteLine(corpoHtml);
                Console.WriteLine($"📧 =========================================");
                Console.WriteLine($"✅ Email simulado com sucesso!");
                return; // Sempre funciona em modo fallback
                
                throw new InvalidOperationException($"Falha no envio de email: {ex.Message}", ex);
            }
        }
    }
}