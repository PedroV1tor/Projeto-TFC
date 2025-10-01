# 🚀 PROJETO TFC - INOVALAB

Sistema completo com Frontend Angular e Backend C# ASP.NET Core.

## 📁 Estrutura do Projeto

```
Projeto TFC/
├── app/                    # Frontend Angular
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/     # Componentes da aplicação
│   │   │   │   ├── header/     # Cabeçalho com menu dinâmico
│   │   │   │   ├── login/      # Tela de login
│   │   │   │   ├── cadastro/   # Tela de cadastro
│   │   │   │   ├── verificar-codigo/  # Verificação de código
│   │   │   │   └── redefinir-senha/   # Redefinição de senha
│   │   │   ├── services/       # Serviços Angular
│   │   │   │   └── auth.service.ts    # Gerenciamento de autenticação
│   │   │   └── app.routes.ts   # Rotas da aplicação
│   │   └── ...
│   ├── package.json
│   └── angular.json
└── backend/               # Backend C# ASP.NET Core
    ├── Controllers/       # Controladores da API
    ├── Services/         # Lógica de negócio
    ├── Models/           # Modelos e DTOs
    ├── Data/             # Contexto do banco
    ├── Program.cs        # Configuração principal
    ├── appsettings.json  # Configurações
    └── start.bat         # Script para iniciar
```

## 🎯 Funcionalidades Implementadas

### Frontend Angular

#### 🔐 Sistema de Autenticação
- **Login completo** com validação
- **Cadastro de usuários** com todos os campos
- **Recuperação de senha** com código por email
- **Redefinição de senha** segura
- **Header dinâmico**: LOGIN ↔ PERFIL

#### 🎨 Interface
- **Design responsivo** para todas as telas
- **Animações suaves** e transições
- **Validação em tempo real** nos formulários
- **Feedback visual** para usuário

#### 🔄 Gerenciamento de Estado
- **Serviço reativo** com Observables
- **Persistência** no localStorage
- **Estado global** sincronizado
- **Auto-login** após refresh da página

### Backend C# ASP.NET Core

#### 🏗️ Arquitetura
- **Clean Architecture** com separação de responsabilidades
- **Dependency Injection** nativo do .NET
- **Entity Framework Core** para acesso a dados
- **Repository Pattern** nos serviços

#### 🔒 Segurança
- **JWT Authentication** com tokens seguros
- **BCrypt** para hash de senhas
- **CORS** configurado para o frontend
- **Validação de entrada** em todos os endpoints

#### 📊 Banco de Dados
- **In-Memory Database** para desenvolvimento
- **Entity Framework Core** com Code First
- **Seed data** com usuários de teste
- **Soft delete** para preservar dados

#### 🌐 API RESTful
- **Endpoints padronizados** REST
- **Swagger/OpenAPI** para documentação
- **Validação automática** com Data Annotations
- **Respostas consistentes** JSON

## 🚀 Como Executar

### Pré-requisitos

- **Node.js** (v18+) e npm
- **.NET 8.0 SDK**
- **Angular CLI** (`npm install -g @angular/cli`)

### 1. Backend (C# ASP.NET Core)

```bash
# Navegar para o backend
cd "Projeto TFC/backend"

# Restaurar pacotes
dotnet restore

# Executar (Opção 1 - Command Line)
dotnet run

# Executar (Opção 2 - Script)
start.bat
```

**URLs do Backend:**
- API: `https://localhost:7000`
- Swagger: `https://localhost:7000/swagger`

### 2. Frontend (Angular)

```bash
# Navegar para o frontend (nova janela do terminal)
cd "Projeto TFC/app"

# Instalar dependências
npm install

# Executar servidor de desenvolvimento
ng serve
```

**URL do Frontend:**
- Aplicação: `http://localhost:4200`

## 👤 Usuários de Teste

O backend vem com usuários pré-cadastrados:

| Email | Senha | Nome |
|-------|-------|------|
| `admin@inovalab.com` | `123456` | Admin Sistema |
| `joao@email.com` | `123456` | João Silva |
| `maria@email.com` | `123456` | Maria Santos |

## 🔄 Fluxo de Funcionamento

### 1. Acesso Inicial
- Usuário acessa `http://localhost:4200`
- Header mostra "LOGIN"
- Pode navegar pelas páginas públicas

### 2. Login
- Clica em "LOGIN" no header
- Preenche email e senha
- Sistema valida credenciais no backend
- Header automaticamente muda para "PERFIL"

### 3. Área Logada
- Menu "PERFIL" com dropdown
- Mostra nome do usuário
- Botão "Sair" para logout
- Estado persiste mesmo após refresh

### 4. Recuperação de Senha
- Na tela de login, clica "Esqueceu a senha?"
- Preenche email
- Recebe código de verificação
- Redefine senha com segurança

### 5. Cadastro
- Na tela de login, clica "Cadastre-se"
- Preenche formulário completo
- Sistema valida e cria conta
- Redireciona para login

## 🛠️ Tecnologias Utilizadas

### Frontend
- **Angular 17** - Framework principal
- **TypeScript** - Linguagem tipada
- **SCSS** - Estilização avançada
- **RxJS** - Programação reativa
- **Angular Router** - Navegação SPA

### Backend
- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core** - ORM
- **JWT Bearer** - Autenticação
- **BCrypt.Net** - Hash de senhas
- **Swagger/OpenAPI** - Documentação
- **In-Memory Database** - Armazenamento

## 🔧 Configurações

### CORS (Backend)
```csharp
// Configurado para aceitar requests do Angular
.WithOrigins("http://localhost:4200")
```

### JWT (Backend)
```json
{
  "JwtSettings": {
    "ExpiryInHours": 24,
    "SecretKey": "ChaveSegura..."
  }
}
```

### Proxy (Frontend)
O Angular faz requests diretamente para `https://localhost:7000`

## 📱 Responsividade

- **Desktop**: Layout otimizado para telas grandes
- **Tablet**: Adapta-se automaticamente
- **Mobile**: Interface totalmente responsiva

## 🎨 Design System

- **Cores principais**: Verde (#12bb20) e Azul (#1556ce)
- **Tipografia**: Poppins para consistência
- **Gradientes**: Usados em botões e fundos
- **Animações**: Transições suaves (0.3s)

## 🚀 Próximos Passos

1. **Produção**:
   - Configurar SQL Server para backend
   - Build otimizado do Angular
   - Deploy em servidor

2. **Funcionalidades**:
   - Página de perfil completa
   - Upload de avatar
   - Dashboard administrativo
   - Relatórios

3. **Melhorias**:
   - Testes unitários
   - Logs estruturados
   - Cache Redis
   - Rate limiting

## 📞 Suporte

Para problemas ou dúvidas:
1. Verificar se ambos os servidores estão rodando
2. Conferir URLs corretas (4200 para Angular, 7000 para API)
3. Verificar console do navegador para erros
4. Checar logs do terminal do backend

---

**Sistema desenvolvido para TFC - INOVALAB** 🎓
