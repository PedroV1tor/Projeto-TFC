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
            
            // Lê a API key do Resend de múltiplas fontes
            var apiKey = configuration["Resend:ApiKey"] 
                ?? configuration["Resend__ApiKey"]
                ?? Environment.GetEnvironmentVariable("Resend__ApiKey")
                ?? "re_U6tGJTfA_96fyCj27fHQQDUtKURrz3HJ3"; // Fallback para a chave fornecida

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Resend API Key não configurada. Configure Resend__ApiKey no .env");
            }

            // Lê o email "From" de múltiplas fontes (apenas Resend)
            _fromEmail = configuration["Resend:FromEmail"] 
                ?? configuration["Resend__FromEmail"]
                ?? Environment.GetEnvironmentVariable("Resend__FromEmail")
                ?? "onboarding@resend.dev"; // Fallback padrão do Resend

            _resend = ResendClient.Create(apiKey);
            
            Console.WriteLine($"✅ ResendEmailService inicializado com From: {_fromEmail}");
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
                Console.WriteLine($"=== ENVIANDO EMAIL COM RESEND ===");
                Console.WriteLine($"De: {_fromEmail}");
                Console.WriteLine($"Para: {destinatarioEmail}");
                Console.WriteLine($"Assunto: {assunto}");

                var emailMessage = new EmailMessage
                {
                    From = _fromEmail,
                    To = destinatarioEmail,
                    Subject = assunto,
                    HtmlBody = corpoHtml
                };

                var resp = await _resend.EmailSendAsync(emailMessage);

                // Se chegou aqui sem exceção, o email foi enviado com sucesso
                Console.WriteLine($"✅ Email enviado com sucesso via Resend!");
                Console.WriteLine($"   Para: {destinatarioEmail}");
                
                // Tenta mostrar o ID se disponível (pode variar conforme versão da biblioteca)
                try
                {
                    if (resp != null)
                    {
                        // A resposta pode ter propriedades diferentes dependendo da versão
                        // Se tiver uma propriedade para o ID, será mostrado
                        var respType = resp.GetType();
                        var idProperty = respType.GetProperty("Id") ?? respType.GetProperty("Data");
                        if (idProperty != null)
                        {
                            var idValue = idProperty.GetValue(resp);
                            if (idValue != null)
                            {
                                Console.WriteLine($"   ID do envio: {idValue}");
                            }
                        }
                    }
                }
                catch
                {
                    // Ignora se não conseguir acessar o ID
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Falha no envio de email para {destinatarioEmail}: {ex.Message}";
                Console.WriteLine($"❌ Erro Resend: {errorMessage}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }
}

