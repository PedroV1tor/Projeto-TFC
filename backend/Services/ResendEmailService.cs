using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Resend;

namespace InovalabAPI.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly IHostEnvironment _environment;
        private readonly string _fromEmail;

        public ResendEmailService(IConfiguration configuration, IHostEnvironment environment)
        {
            _environment = environment;
            
            var apiKey = configuration["Resend:ApiKey"] 
                ?? configuration["Resend__ApiKey"]
                ?? Environment.GetEnvironmentVariable("Resend__ApiKey")
                ?? "re_U6tGJTfA_96fyCj27fHQQDUtKURrz3HJ3";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Resend API Key não configurada. Configure Resend__ApiKey no .env");
            }

            _fromEmail = configuration["Resend:FromEmail"] 
                ?? configuration["Resend__FromEmail"]
                ?? Environment.GetEnvironmentVariable("Resend__FromEmail")
                ?? "onboarding@resend.dev";

            _resend = ResendClient.Create(apiKey);
        }

        public async Task EnviarAsync(string destinatarioEmail, string assunto, string corpoHtml)
        {
            if (string.IsNullOrWhiteSpace(destinatarioEmail))
            {
                throw new ArgumentException("Destinatário do email não pode ser vazio", nameof(destinatarioEmail));
            }

            if (string.IsNullOrWhiteSpace(assunto))
            {
                throw new ArgumentException("Assunto do email não pode ser vazio", nameof(assunto));
            }

            try
            {
                var emailMessage = new EmailMessage
                {
                    From = _fromEmail,
                    To = destinatarioEmail,
                    Subject = assunto,
                    HtmlBody = corpoHtml
                };

                await _resend.EmailSendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Falha no envio de email para {destinatarioEmail}: {ex.Message}";
                
                if (errorMessage.Contains("You can only send testing emails") || 
                    errorMessage.Contains("verify a domain"))
                {
                    throw new InvalidOperationException(
                        $"Não é possível enviar email para {destinatarioEmail}. " +
                        $"O Resend está em modo de teste. " +
                        $"Verifique um domínio em resend.com/domains para enviar para qualquer destinatário.", ex);
                }
                
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }
}

