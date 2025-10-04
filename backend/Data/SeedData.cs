using InovalabAPI.Models;
using BCrypt.Net;

namespace InovalabAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();


            if (context.Usuarios.Any())
            {
                return; // Banco já foi populado
            }


            var usuarios = new Usuario[]
            {
                new Usuario
                {
                    Nome = "Admin",
                    Sobrenome = "Sistema",
                    Email = "admin@inovalab.com",
                    NomeUsuario = "admin",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Telefone = "(11) 99999-9999",
                    CEP = "01000-000",
                    Rua = "Rua da Administração",
                    Bairro = "Centro",
                    Numero = "100",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                },
                new Usuario
                {
                    Nome = "João",
                    Sobrenome = "Silva",
                    Email = "joao@email.com",
                    NomeUsuario = "joao.silva",
                    Matricula = "2024001",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Telefone = "(11) 98888-8888",
                    CEP = "02000-000",
                    Rua = "Rua das Flores",
                    Bairro = "Jardim",
                    Numero = "200",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                },
                new Usuario
                {
                    Nome = "Maria",
                    Sobrenome = "Santos",
                    Email = "maria@email.com",
                    NomeUsuario = "maria.santos",
                    Matricula = "2024002",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Telefone = "(11) 97777-7777",
                    CEP = "03000-000",
                    Rua = "Avenida Principal",
                    Bairro = "Vila Nova",
                    Numero = "300",
                    Complemento = "Apto 101",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                }
            };

            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();
        }
    }
}
