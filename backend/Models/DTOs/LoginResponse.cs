namespace InovalabAPI.Models.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
