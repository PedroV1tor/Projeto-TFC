using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models
{
    public class Publicacao
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Resumo { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Autor { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(ativa|rascunho|arquivada)$")]
        public string Status { get; set; } = "ativa";

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public DateTime? AtualizadoEm { get; set; }

        public int Visualizacoes { get; set; } = 0;

        public int Curtidas { get; set; } = 0;

        // Relacionamento com usuário
        public int UsuarioId { get; set; }
        public virtual Usuario UsuarioCriador { get; set; } = null!;
    }
}
