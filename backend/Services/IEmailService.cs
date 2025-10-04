using System.Threading.Tasks;

namespace InovalabAPI.Services
{
    public interface IEmailService
    {
        Task EnviarAsync(string destinatarioEmail, string assunto, string corpoHtml);
    }
}
