using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class OrcamentoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime PrazoEntrega { get; set; }
        public DateTime PrazoOrcamento { get; set; }
        public decimal? Valor { get; set; }
        public string? Cliente { get; set; }
        public string? Responsavel { get; set; }
        public string Status { get; set; } = "Pendente";
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }

    public class CriarOrcamentoDTO
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O prazo de entrega é obrigatório")]
        public DateTime PrazoEntrega { get; set; }

        [Required(ErrorMessage = "O prazo do orçamento é obrigatório")]
        public DateTime PrazoOrcamento { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal? Valor { get; set; }

        [StringLength(100, ErrorMessage = "O nome do cliente deve ter no máximo 100 caracteres")]
        public string? Cliente { get; set; }

        [StringLength(100, ErrorMessage = "O nome do responsável deve ter no máximo 100 caracteres")]
        public string? Responsavel { get; set; }


    }

    public class AtualizarOrcamentoDTO
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O prazo de entrega é obrigatório")]
        public DateTime PrazoEntrega { get; set; }

        [Required(ErrorMessage = "O prazo do orçamento é obrigatório")]
        public DateTime PrazoOrcamento { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal? Valor { get; set; }

        [StringLength(100, ErrorMessage = "O nome do cliente deve ter no máximo 100 caracteres")]
        public string? Cliente { get; set; }

        [StringLength(100, ErrorMessage = "O nome do responsável deve ter no máximo 100 caracteres")]
        public string? Responsavel { get; set; }


    }

    public class AlterarStatusOrcamentoDTO
    {
        [Required(ErrorMessage = "O status é obrigatório")]
        [RegularExpression("^(Pendente|Aprovado|Rejeitado|Concluido)$", ErrorMessage = "Status deve ser: Pendente, Aprovado, Rejeitado ou Concluido")]
        public string Status { get; set; } = string.Empty;
    }

    public class EstatisticasOrcamentoDTO
    {
        public int Total { get; set; }
        public int Pendentes { get; set; }
        public int Aprovados { get; set; }
        public int Rejeitados { get; set; }
        public int Concluidos { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorMedio { get; set; }
        public List<OrcamentoResumoDTO> OrcamentosVencendo { get; set; } = new List<OrcamentoResumoDTO>();
    }

    public class OrcamentoResumoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Cliente { get; set; }
        public DateTime PrazoOrcamento { get; set; }
        public DateTime PrazoEntrega { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DiasRestantes { get; set; }
        public bool PrazoVencido { get; set; }
    }

    public class FiltroOrcamentoDTO
    {
        public string? Status { get; set; }
        public string? Cliente { get; set; }
        public string? Responsavel { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public bool? PrazosVencidos { get; set; }
        public int? DiasPrazo { get; set; }
    }
}
