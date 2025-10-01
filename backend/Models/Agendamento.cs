using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models
{
    public class Agendamento
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Usuario { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(pendente|aprovado|reprovado)$")]
        public string Status { get; set; } = "pendente";

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public DateTime? AtualizadoEm { get; set; }

        // Relacionamento com usuário
        public int UsuarioId { get; set; }
        public virtual Usuario UsuarioCriador { get; set; } = null!;
    }
}
