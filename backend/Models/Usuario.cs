using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Sobrenome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Matricula { get; set; }

        [Required]
        [StringLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? UltimoLogin { get; set; }

        public bool Ativo { get; set; } = true;

        public bool IsAdmin { get; set; } = false;

        // Relacionamento com EnderecoUsuario
        public EnderecoUsuario? Endereco { get; set; }
    }
}
