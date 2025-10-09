using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string RazaoSocial { get; set; } = string.Empty;

        [StringLength(200)]
        public string? NomeFantasia { get; set; }

        [Required]
        [StringLength(18)]
        public string CNPJ { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ResponsavelNome { get; set; }

        [StringLength(20)]
        public string? ResponsavelTelefone { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? UltimoLogin { get; set; }

        public bool Ativo { get; set; } = true;

        // Relacionamento com EnderecoEmpresa
        public EnderecoEmpresa? Endereco { get; set; }
    }
}

