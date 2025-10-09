using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models
{
    public class EnderecoEmpresa
    {
        public int Id { get; set; }

        [Required]
        public int EmpresaId { get; set; }

        [Required]
        [StringLength(10)]
        public string CEP { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Rua { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Numero { get; set; }

        [StringLength(255)]
        public string? Referencia { get; set; }

        [StringLength(100)]
        public string? Complemento { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Relacionamento com Empresa
        public Empresa Empresa { get; set; } = null!;
    }
}

