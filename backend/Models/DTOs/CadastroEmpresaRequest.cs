using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class CadastroEmpresaRequest
    {
        [Required(ErrorMessage = "A razão social é obrigatória")]
        [StringLength(200, ErrorMessage = "A razão social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "O nome fantasia deve ter no máximo 200 caracteres")]
        public string? NomeFantasia { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres")]
        public string CNPJ { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O nome do responsável deve ter no máximo 100 caracteres")]
        public string? ResponsavelNome { get; set; }

        [StringLength(20, ErrorMessage = "O telefone do responsável deve ter no máximo 20 caracteres")]
        public string? ResponsavelTelefone { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        public EnderecoEmpresaDTO Endereco { get; set; } = new EnderecoEmpresaDTO();
    }
}

