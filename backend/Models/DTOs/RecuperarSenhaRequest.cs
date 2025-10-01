using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class RecuperarSenhaRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class VerificarCodigoRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 5)]
        public string Codigo { get; set; } = string.Empty;
    }

    public class RedefinirSenhaRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 5)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NovaSenha { get; set; } = string.Empty;
    }
}
