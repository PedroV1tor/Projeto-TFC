using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models
{
    public class Orcamento
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public DateTime PrazoEntrega { get; set; }

        [Required]
        public DateTime PrazoOrcamento { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Valor { get; set; }

        [StringLength(100)]
        public string? Cliente { get; set; }

        [StringLength(100)]
        public string? Responsavel { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(pendente|aprovado|rejeitado|concluido)$")]
        public string Status { get; set; } = "pendente";

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public DateTime? AtualizadoEm { get; set; }


        public int UsuarioId { get; set; }
        public virtual Usuario UsuarioCriador { get; set; } = null!;
    }
}
