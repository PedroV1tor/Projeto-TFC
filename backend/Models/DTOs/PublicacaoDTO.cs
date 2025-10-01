using System.ComponentModel.DataAnnotations;

namespace InovalabAPI.Models.DTOs
{
    public class PublicacaoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Autor { get; set; }
        public string Status { get; set; } = "Ativa";
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public int Visualizacoes { get; set; }
        public int Curtidas { get; set; }
    }

    public class CriarPublicacaoDTO
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O resumo é obrigatório")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "O resumo deve ter entre 10 e 500 caracteres")]
        public string Resumo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(5000, MinimumLength = 50, ErrorMessage = "A descrição deve ter entre 50 e 5000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O nome do autor deve ter no máximo 100 caracteres")]
        public string? Autor { get; set; }
    }

    public class AtualizarPublicacaoDTO
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O resumo é obrigatório")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "O resumo deve ter entre 10 e 500 caracteres")]
        public string Resumo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(5000, MinimumLength = 50, ErrorMessage = "A descrição deve ter entre 50 e 5000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O nome do autor deve ter no máximo 100 caracteres")]
        public string? Autor { get; set; }
    }

    public class AlterarStatusDTO
    {
        [Required(ErrorMessage = "O status é obrigatório")]
        public string Status { get; set; } = string.Empty;
    }

    public class PublicacaoTimelineDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Autor { get; set; }
        public DateTime CriadoEm { get; set; }
        public int Visualizacoes { get; set; }
        public int Curtidas { get; set; }
        public string TempoRelativo { get; set; } = string.Empty;
    }
}
