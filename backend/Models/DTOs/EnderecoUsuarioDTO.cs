using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class EnderecoUsuarioDTO
    {
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
        public string CEP { get; set; } = string.Empty;

        [Required(ErrorMessage = "A rua é obrigatória")]
        [StringLength(255, ErrorMessage = "A rua deve ter no máximo 255 caracteres")]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório")]
        [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres")]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string? Numero { get; set; }

        [StringLength(255, ErrorMessage = "A referência deve ter no máximo 255 caracteres")]
        public string? Referencia { get; set; }

        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complemento { get; set; }
    }
}

