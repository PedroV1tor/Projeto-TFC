using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class AgendamentoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Usuario { get; set; }
        public string Status { get; set; } = "pendente";
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }

    public class CriarAgendamentoDTO
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O nome do usuário deve ter no máximo 100 caracteres")]
        public string? Usuario { get; set; }
    }

    public class AtualizarAgendamentoDTO
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O nome do usuário deve ter no máximo 100 caracteres")]
        public string? Usuario { get; set; }
    }

    public class AlterarStatusAgendamentoDTO
    {
        [Required(ErrorMessage = "O status é obrigatório")]
        [RegularExpression("^(pendente|aprovado|reprovado)$", ErrorMessage = "Status deve ser: pendente, aprovado ou reprovado")]
        public string Status { get; set; } = string.Empty;
    }

    public class FiltroAgendamentoDTO
    {
        public string? Status { get; set; }
        public string? Usuario { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool? ProximosEventos { get; set; }
        public int? ProximosDias { get; set; }
    }
}
