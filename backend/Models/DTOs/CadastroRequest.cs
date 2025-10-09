using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class CadastroRequest
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Sobrenome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Matricula { get; set; }

        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório")]
        public EnderecoUsuarioDTO Endereco { get; set; } = new EnderecoUsuarioDTO();
    }
}
