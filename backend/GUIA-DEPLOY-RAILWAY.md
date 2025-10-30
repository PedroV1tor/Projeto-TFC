# 🚂 Deploy Backend C# no Railway - Guia Rápido

## ✅ Por que Railway?

- ✅ Suporte nativo para .NET Core
- ✅ Plano gratuito generoso ($5 grátis/mês)
- ✅ Deploy em minutos
- ✅ Fácil configuração
- ✅ PostgreSQL incluído
- ✅ HTTPS automático

## 🚀 Deploy em 5 Passos

### 1. Instalar Railway CLI

**Windows (PowerShell como Admin):**
```powershell
iwr https://railway.app/install.ps1 | iex
```

**Node.js (Alternativa):**
```bash
npm install -g @railway/cli
```

### 2. Login

```bash
railway login
```
Abra o navegador e faça login com GitHub.

### 3. Navegar para pasta backend

```bash
cd backend
```

### 4. Inicializar projeto

```bash
railway init
```

Quando perguntado:
- **"New Project"** → Digite o nome (ex: inovalab-api)
- Confirme

### 5. Deploy

```bash
railway up
```

O Railway vai:
1. Fazer build do .NET
2. Criar container Docker
3. Deploy automático
4. Gerar URL pública

## ⚙️ Configurar Variáveis de Ambiente

### Opção 1: Via CLI

```bash
railway variables
```

### Opção 2: Via Painel Web

1. Acesse https://railway.app
2. Clique no projeto
3. Abra "Variables"
4. Adicione as variáveis:

**Variáveis necessárias:**

```env
ASPNETCORE_ENVIRONMENT=Production

ConnectionStrings__DefaultConnection=Host=db.ykuhkuphxsrrqhvbooti.supabase.co;Database=postgres;Username=postgres;Password=P3droV019@;SSL Mode=Require;Trust Server Certificate=true

JwtSettings__SecretKey=MinhaChaveSecretaSuperSeguraComMaisDe32Caracteres123456

JwtSettings__Issuer=InovalabAPI

JwtSettings__Audience=InovalabApp

JwtSettings__ExpiryInHours=24

ProductionUrl=https://seu-frontend.vercel.app

EmailSettings__Host=smtp.gmail.com

EmailSettings__Port=587

EmailSettings__EnableSsl=true

EmailSettings__User=pedrovitormarques019@gmail.com

EmailSettings__Password=sgwhudiuuamygtte

EmailSettings__From=pedrovitormarques019@gmail.com
```

## 📝 Passos Pós-Deploy

### 1. Obter URL da API

```bash
railway domain
```

Ou no painel Railway, veja "Settings" → "Custom Domain"

A URL será algo como: `https://inovalab-api.railway.app`

### 2. Atualizar CORS no Program.cs

Edite `Program.cs` e adicione sua URL:

```csharp
var allowedOrigins = new List<string>
{
    "http://localhost:4200",
    "https://seu-frontend.vercel.app"  // Adicione a URL do Railway aqui
};
```

### 3. Atualizar Frontend

Edite `app/src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://sua-api.railway.app/api'  // URL do Railway
};
```

### 4. Redeploy

```bash
railway up
```

## 🔧 Comandos Úteis

```bash
# Ver logs em tempo real
railway logs

# Ver status
railway status

# Ver URL
railway domain

# Abrir painel web
railway open

# Ver variáveis
railway variables

# Executar comando no container
railway run dotnet ef database update

# Ver métricas
railway metrics
```

## 📊 Comandos Avançados

### Adicionar PostgreSQL do Railway (Opcional)

Se quiser usar PostgreSQL do Railway em vez de Supabase:

```bash
# Adicionar PostgreSQL
railway add postgresql

# Ver connection string
railway variables | grep DATABASE_URL

# Atualizar appsettings.Production.json com a nova URL
```

### Configurar Domínio Personalizado

1. No painel Railway → Settings
2. Custom Domains
3. Adicione seu domínio
4. Configure DNS conforme instruções

## ⚠️ Troubleshooting

### Build Failing

```bash
# Ver logs detalhados
railway logs --deployment

# Verificar build localmente
dotnet build -c Release
```

### Database Connection Error

Verifique se:
1. Variável `ConnectionStrings__DefaultConnection` está correta
2. Supabase permite conexões externas
3. Credenciais estão corretas

### CORS Error

1. Verifique `ProductionUrl` nas variáveis
2. Confira se a URL está na lista de origens em `Program.cs`
3. Redeploy após mudanças

## 💰 Preços

**Gratuito:**
- $5/mês de créditos
- 500 horas de execução
- 100GB de egress por mês

**Para projetos maiores:**
- Hobby: $5/mês
- Pro: $20/mês

## 📚 Documentação

- Railway Docs: https://docs.railway.app
- .NET Deploy: https://docs.railway.app/languages/dotnet

## ✅ Checklist Final

- [ ] Railway CLI instalado
- [ ] Login feito (`railway login`)
- [ ] Projeto inicializado (`railway init`)
- [ ] Primeiro deploy feito (`railway up`)
- [ ] Variáveis de ambiente configuradas
- [ ] CORS configurado
- [ ] URL da API anotada
- [ ] Frontend atualizado com nova URL
- [ ] Testado login/API
- [ ] Domínio personalizado (opcional)

## 🎯 Resultado Esperado

Após o deploy, você terá:
- ✅ API funcionando em: `https://sua-api.railway.app`
- ✅ HTTPS automático
- ✅ Deploy contínuo (git push)
- ✅ Logs em tempo real
- ✅ Métricas e monitoramento

