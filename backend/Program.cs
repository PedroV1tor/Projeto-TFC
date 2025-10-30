using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InovalabAPI.Data;
using InovalabAPI.Services;
using System.Text.Json;
using System.Collections.Generic;

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

      /*options.AddPolicy("AllowDevelopment", policy =>
      {
          policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
      });*/
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
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

var app = builder.Build();

// ========================================
// CORS precisa vir logo no início do pipeline
// ========================================
app.UseCors(policy =>
{
    policy.WithOrigins("https://frontend-production-0b8e.up.railway.app")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
});

// ========================================
// Swagger só no modo desenvolvimento
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ========================================
// HTTPS Redirection vem depois do CORS
// ========================================
app.UseHttpsRedirection();

// ========================================
// Autenticação e Autorização
// ========================================
app.UseAuthentication();
app.UseAuthorization();

// ========================================
// Controllers
// ========================================
app.MapControllers();

// ========================================
// Migrações e Seed
// ========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    SeedData.Initialize(context);
}

app.Run();
