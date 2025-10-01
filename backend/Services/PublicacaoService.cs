using Microsoft.EntityFrameworkCore;
using InovalabAPI.Data;
using InovalabAPI.Models;
using InovalabAPI.Models.DTOs;

namespace InovalabAPI.Services
{
    public class PublicacaoService : IPublicacaoService
    {
        private readonly ApplicationDbContext _context;

        public PublicacaoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PublicacaoDTO>> GetAllAsync()
        {
            var publicacoes = await _context.Publicacoes
                .Include(p => p.UsuarioCriador)
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            return publicacoes.Select(p => MapToDTO(p));
        }

        public async Task<PublicacaoDTO?> GetByIdAsync(int id)
        {
            var publicacao = await _context.Publicacoes
                .Include(p => p.UsuarioCriador)
                .FirstOrDefaultAsync(p => p.Id == id);

            return publicacao != null ? MapToDTO(publicacao) : null;
        }

        public async Task<PublicacaoDTO> CreateAsync(CriarPublicacaoDTO criarPublicacaoDto, int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            var publicacao = new Publicacao
            {
                Titulo = criarPublicacaoDto.Titulo,
                Resumo = criarPublicacaoDto.Resumo,
                Descricao = criarPublicacaoDto.Descricao,
                Autor = criarPublicacaoDto.Autor ?? $"{usuario.Nome} {usuario.Sobrenome}",
                Status = "ativa",
                UsuarioId = usuarioId,
                CriadoEm = DateTime.UtcNow,
                Visualizacoes = 0,
                Curtidas = 0
            };

            _context.Publicacoes.Add(publicacao);
            await _context.SaveChangesAsync();

            publicacao.UsuarioCriador = usuario;
            return MapToDTO(publicacao);
        }

        public async Task<bool> UpdateAsync(int id, AtualizarPublicacaoDTO atualizarPublicacaoDto)
        {
            var publicacao = await _context.Publicacoes.FindAsync(id);
            if (publicacao == null)
                return false;

            publicacao.Titulo = atualizarPublicacaoDto.Titulo;
            publicacao.Resumo = atualizarPublicacaoDto.Resumo;
            publicacao.Descricao = atualizarPublicacaoDto.Descricao;
            publicacao.Autor = atualizarPublicacaoDto.Autor ?? publicacao.Autor;
            publicacao.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var publicacao = await _context.Publicacoes.FindAsync(id);
            if (publicacao == null)
                return false;

            _context.Publicacoes.Remove(publicacao);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarStatusAsync(int id, AlterarStatusDTO alterarStatusDto)
        {
            var publicacao = await _context.Publicacoes.FindAsync(id);
            if (publicacao == null)
                return false;

            publicacao.Status = alterarStatusDto.Status;
            publicacao.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PublicacaoDTO>> GetAtivasAsync()
        {
            var publicacoes = await _context.Publicacoes
                .Include(p => p.UsuarioCriador)
                .Where(p => p.Status == "ativa")
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            return publicacoes.Select(p => MapToDTO(p));
        }

        public async Task<IEnumerable<PublicacaoTimelineDTO>> GetTimelineAsync()
        {
            var publicacoes = await _context.Publicacoes
                .Include(p => p.UsuarioCriador)
                .Where(p => p.Status == "ativa")
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            return publicacoes.Select(p => MapToTimelineDTO(p));
        }

        public async Task<bool> IncrementarVisualizacoesAsync(int id)
        {
            var publicacao = await _context.Publicacoes.FindAsync(id);
            if (publicacao == null)
                return false;

            publicacao.Visualizacoes++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CurtirPublicacaoAsync(int id)
        {
            var publicacao = await _context.Publicacoes.FindAsync(id);
            if (publicacao == null)
                return false;

            publicacao.Curtidas++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PublicacaoDTO>> GetByUsuarioAsync(int usuarioId)
        {
            var publicacoes = await _context.Publicacoes
                .Include(p => p.UsuarioCriador)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            return publicacoes.Select(p => MapToDTO(p));
        }

        public async Task<IEnumerable<PublicacaoDTO>> GetByStatusAsync(string status)
        {
            var publicacoes = await _context.Publicacoes
                .Include(p => p.UsuarioCriador)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            return publicacoes.Select(p => MapToDTO(p));
        }

        private static PublicacaoDTO MapToDTO(Publicacao publicacao)
        {
            return new PublicacaoDTO
            {
                Id = publicacao.Id,
                Titulo = publicacao.Titulo,
                Resumo = publicacao.Resumo,
                Descricao = publicacao.Descricao,
                Autor = publicacao.Autor,
                Status = publicacao.Status,
                CriadoEm = publicacao.CriadoEm,
                AtualizadoEm = publicacao.AtualizadoEm,
                Visualizacoes = publicacao.Visualizacoes,
                Curtidas = publicacao.Curtidas
            };
        }

        private static PublicacaoTimelineDTO MapToTimelineDTO(Publicacao publicacao)
        {
            var tempoDecorrido = DateTime.UtcNow - publicacao.CriadoEm;
            string tempoRelativo;

            if (tempoDecorrido.TotalMinutes < 60)
                tempoRelativo = $"há {(int)tempoDecorrido.TotalMinutes} minutos";
            else if (tempoDecorrido.TotalHours < 24)
                tempoRelativo = $"há {(int)tempoDecorrido.TotalHours} horas";
            else if (tempoDecorrido.TotalDays < 30)
                tempoRelativo = $"há {(int)tempoDecorrido.TotalDays} dias";
            else
                tempoRelativo = publicacao.CriadoEm.ToString("dd/MM/yyyy");

            return new PublicacaoTimelineDTO
            {
                Id = publicacao.Id,
                Titulo = publicacao.Titulo,
                Resumo = publicacao.Resumo,
                Descricao = publicacao.Descricao,
                Autor = publicacao.Autor,
                CriadoEm = publicacao.CriadoEm,
                Visualizacoes = publicacao.Visualizacoes,
                Curtidas = publicacao.Curtidas,
                TempoRelativo = tempoRelativo
            };
        }
    }
}
