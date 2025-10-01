namespace InovalabAPI.Models.DTOs
{
    // DTO base para respostas de relatórios
    public class RelatorioResponseDTO<T>
    {
        public int Total { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime GeradoEm { get; set; }
    }

    // DTO para relatório de usuários
    public class RelatorioUsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
    }

    // DTO para relatório de publicações
    public class RelatorioPublicacaoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Visualizacoes { get; set; }
        public int Curtidas { get; set; }
        public DateTime CriadoEm { get; set; }
    }

    // DTO para relatório de agendamentos
    public class RelatorioAgendamentoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
    }

    // DTO para relatório de orçamentos
    public class RelatorioOrcamentoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
    }

    // DTO para resumo geral
    public class RelatorioResumoDTO
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int TotalPublicacoes { get; set; }
        public int PublicacoesAtivas { get; set; }
        public int TotalAgendamentos { get; set; }
        public int AgendamentosAtivos { get; set; }
        public int TotalOrcamentos { get; set; }
        public int OrcamentosAprovados { get; set; }
        public decimal ValorTotalOrcamentos { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime GeradoEm { get; set; }
    }
}
