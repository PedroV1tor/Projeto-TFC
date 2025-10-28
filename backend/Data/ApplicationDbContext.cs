using Microsoft.EntityFrameworkCore;
using InovalabAPI.Models;

namespace InovalabAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EnderecoUsuario> EnderecosUsuario { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<EnderecoEmpresa> EnderecosEmpresa { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Publicacao> Publicacoes { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.NomeUsuario).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.SenhaHash).IsRequired();
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Sobrenome).IsRequired();
                entity.Property(e => e.NomeUsuario).IsRequired();
                entity.Property(e => e.Telefone).IsRequired();

                entity.HasOne(e => e.Endereco)
                      .WithOne(e => e.Usuario)
                      .HasForeignKey<EnderecoUsuario>(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EnderecoUsuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CEP).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Rua).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Numero).HasMaxLength(10);
                entity.Property(e => e.Referencia).HasMaxLength(255);
                entity.Property(e => e.Complemento).HasMaxLength(100);
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.CNPJ).IsUnique();
                entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NomeFantasia).HasMaxLength(200);
                entity.Property(e => e.CNPJ).IsRequired().HasMaxLength(18);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ResponsavelNome).HasMaxLength(100);
                entity.Property(e => e.ResponsavelTelefone).HasMaxLength(20);

                entity.HasOne(e => e.Endereco)
                      .WithOne(e => e.Empresa)
                      .HasForeignKey<EnderecoEmpresa>(e => e.EmpresaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EnderecoEmpresa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CEP).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Rua).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Numero).HasMaxLength(10);
                entity.Property(e => e.Referencia).HasMaxLength(255);
                entity.Property(e => e.Complemento).HasMaxLength(100);
            });


            modelBuilder.Entity<Agendamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Data).IsRequired();
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Usuario).HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CriadoEm).IsRequired();

                entity.HasOne(e => e.UsuarioCriador)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Publicacao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Resumo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(5000);
                entity.Property(e => e.Autor).HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CriadoEm).IsRequired();

                entity.HasOne(e => e.UsuarioCriador)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Orcamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.PrazoEntrega).IsRequired();
                entity.Property(e => e.PrazoOrcamento).IsRequired();
                entity.Property(e => e.Valor).HasPrecision(18, 2);
                entity.Property(e => e.Cliente).HasMaxLength(100);
                entity.Property(e => e.Responsavel).HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CriadoEm).IsRequired();

                entity.HasOne(e => e.UsuarioCriador)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
