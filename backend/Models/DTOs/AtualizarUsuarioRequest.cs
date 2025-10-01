using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class AtualizarUsuarioRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "Sobrenome deve ter no máximo 100 caracteres")]
        public string Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "CEP é obrigatório")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato 00000-000")]
        public string CEP { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rua é obrigatória")]
        [StringLength(200, ErrorMessage = "Rua deve ter no máximo 200 caracteres")]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bairro é obrigatório")]
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(10, ErrorMessage = "Número deve ter no máximo 10 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Referência deve ter no máximo 100 caracteres")]
        public string? Referencia { get; set; }

        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string? Complemento { get; set; }
    }
}

