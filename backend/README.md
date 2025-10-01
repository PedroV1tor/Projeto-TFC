# INOVALAB API - Backend C#

Backend desenvolvido em ASP.NET Core 8.0 para o sistema INOVALAB.

## Características

- **ASP.NET Core 8.0** - Framework web moderno
- **Entity Framework Core** - ORM para acesso a dados
- **JWT Authentication** - Autenticação segura
- **BCrypt** - Hash seguro de senhas
- **Swagger/OpenAPI** - Documentação da API
- **CORS** - Suporte para aplicações frontend
- **In-Memory Database** - Para desenvolvimento e testes

## Estrutura do Projeto

```
backend/
├── Controllers/           # Controladores da API
│   ├── AuthController.cs # Autenticação e cadastro
│   └── UserController.cs # Gerenciamento de usuários
├── Data/                 # Contexto do banco de dados
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs      # Dados iniciais
├── Models/              # Modelos de dados
│   ├── Usuario.cs       # Modelo do usuário
│   └── DTOs/            # Data Transfer Objects
├── Services/            # Serviços de negócio
│   ├── AuthService.cs   # Lógica de autenticação
│   └── UserService.cs   # Lógica de usuários
├── Program.cs           # Configuração da aplicação
└── appsettings.json     # Configurações
```

## Endpoints da API

### Autenticação (`/api/auth`)

- `POST /api/auth/login` - Login do usuário
- `POST /api/auth/cadastro` - Cadastro de novo usuário
- `POST /api/auth/recuperar-senha` - Solicitar recuperação de senha
- `POST /api/auth/verificar-codigo` - Verificar código de recuperação
- `POST /api/auth/redefinir-senha` - Redefinir senha

### Usuários (`/api/user`)

- `GET /api/user/perfil` - Obter perfil do usuário logado
- `GET /api/user/todos` - Listar todos os usuários
- `DELETE /api/user/{id}` - Remover usuário

## Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- Visual Studio, VS Code ou outro editor

### Passos

1. **Navegar para o diretório do backend:**
   ```bash
   cd backend
   ```

2. **Restaurar pacotes:**
   ```bash
   dotnet restore
   ```

3. **Executar a aplicação:**
   ```bash
   dotnet run
   ```

4. **Acessar a API:**
   - API: `https://localhost:7000` ou `http://localhost:5000`
   - Swagger: `https://localhost:7000/swagger`

## Configurações

### JWT Settings (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "MinhaChaveSecretaSuperSeguraComMaisDe32Caracteres123456",
    "Issuer": "InovalabAPI",
    "Audience": "InovalabApp",
    "ExpiryInHours": 24
  }
}
```

### CORS

Configurado para aceitar requests do frontend Angular (`http://localhost:4200`).

## Usuários de Teste

O sistema vem com usuários pré-cadastrados:

1. **Admin:**
   - Email: `admin@inovalab.com`
   - Senha: `123456`

2. **João Silva:**
   - Email: `joao@email.com`
   - Senha: `123456`

3. **Maria Santos:**
   - Email: `maria@email.com`
   - Senha: `123456`

## Funcionalidades Implementadas

### Autenticação
- Login com email e senha
- Geração de JWT tokens
- Validação de credenciais com BCrypt

### Cadastro
- Cadastro completo de usuários
- Validação de emails únicos
- Hash seguro de senhas

### Recuperação de Senha
- Geração de códigos de verificação
- Verificação de códigos
- Redefinição segura de senhas

### Gerenciamento de Usuários
- Consulta de perfil
- Listagem de usuários
- Soft delete de usuários

## Segurança

- **Senhas hasheadas** com BCrypt
- **JWT tokens** para autenticação
- **Validação de entrada** com Data Annotations
- **CORS** configurado para origem específica
- **HTTPS** habilitado por padrão

## Desenvolvimento

Para desenvolvimento local, o banco de dados em memória é usado automaticamente. Para produção, configure a connection string no `appsettings.json`.

## Próximos Passos

- Configurar banco de dados SQL Server para produção
- Implementar envio real de emails para recuperação de senha
- Adicionar logs estruturados
- Implementar cache Redis para códigos de recuperação
- Adicionar testes unitários e de integração

