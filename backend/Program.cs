using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InovalabAPI.Data;
using InovalabAPI.Services;
using System.Text.Json;
using System.Collections.Generic;
using DotNetEnv;

// Carrega variáveis de ambiente do arquivo .env
// O .env deve estar no diretório raiz do projeto backend
// Usa múltiplos caminhos possíveis para encontrar o .env
var currentDir = Directory.GetCurrentDirectory();
var envPaths = new[]
{
    Path.Combine(currentDir, ".env"),
    Path.Combine(currentDir, "backend", ".env"),
    Path.Combine(AppContext.BaseDirectory, ".env"),
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env"), // Para executáveis compilados
    ".env" // Caminho relativo padrão
};

string? envPath = null;
foreach (var path in envPaths)
{
    if (File.Exists(path))
    {
        envPath = Path.GetFullPath(path);
        break;
    }
}

if (envPath != null)
{
    Env.Load(envPath);
    Console.WriteLine($"✅ Arquivo .env carregado de: {envPath}");
}
else
{
    Console.WriteLine($"⚠️ Arquivo .env não encontrado. Procurou em:");
    foreach (var path in envPaths)
    {
        Console.WriteLine($"   - {Path.GetFullPath(path)}");
    }
}

// IMPORTANTE: Env.Load() deve ser chamado ANTES do WebApplication.CreateBuilder
// porque o builder já carrega variáveis de ambiente automaticamente
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
      options.AddPolicy(name: "AllowProduction",
       policy =>    {
          policy.WithOrigins("https://frontend-production-0b8e.up.railway.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
      });

      options.AddPolicy("AllowDevelopment", policy =>
      {
          policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
      });
      });

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "MinhaChaveSecretaSuperSeguraComMaisDe32Caracteres123456";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "InovalabAPI",
            ValidAudience = jwtSettings["Audience"] ?? "InovalabApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IPublicacaoService, PublicacaoService>();
builder.Services.AddScoped<IOrcamentoService, OrcamentoService>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();

var app = builder.Build();

// --- CONFIGURAÇÃO DE CORS ---
// Ativa CORS baseado no ambiente
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowDevelopment");
}
else
{
    app.UseCors("AllowProduction");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    SeedData.Initialize(context);
}
// Em produção, usa a porta do Railway, em desenvolvimento usa a do launchSettings.json
if (!app.Environment.IsDevelopment())
{
    app.Urls.Add("http://0.0.0.0:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));
}

app.Run();
