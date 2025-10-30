# 🚀 Redeploy do Backend - CORS Corrigido

## ✅ O que foi corrigido

1. **Middleware personalizado para CORS** - Captura requests OPTIONS (preflight)
2. **Ordem correta dos middlewares** - CORS antes de UseHttpsRedirection
3. **Origem dinâmica** - Aceita a origem da requisição automaticamente

## 🔧 Redeployar o Backend

### Via Vercel CLI

```bash
cd backend
vercel --prod
```

### Via Railway (se estiver usando)

```bash
cd backend
railway up
```

## ⏱️ Aguardar o Deploy

O redeploy pode levar 2-5 minutos. Você verá algo como:

```
✓ Deploy complete: https://projeto-tfc-2uh9.vercel.app
```

## 🧪 Testar

Após o deploy, teste fazendo login:

1. Acesse: https://frontendtfc.vercel.app
2. F12 → Console
3. Tente fazer login
4. **Não deve mais ter erro de CORS**

## 🔍 Verificar se Funcionou

Se ainda der erro de CORS, verifique:

1. **Backend está rodando?**
   - Acesse: https://projeto-tfc-2uh9.vercel.app/api
   - Deve mostrar swagger ou resposta da API

2. **Headers CORS na resposta?**
   - F12 → Network → Clique na requisição
   - Deve ver:
     - `Access-Control-Allow-Origin: https://frontendtfc.vercel.app`
     - `Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS`
     - `Access-Control-Allow-Credentials: true`

3. **Logs do backend?**
   ```bash
   vercel logs
   ```

## 📝 Código Aplicado

```csharp
// Middleware personalizado para tratar CORS (deve ser o primeiro)
app.Use(async (context, next) =>
{
    // Obter a origem da requisição
    var origin = context.Request.Headers["Origin"].ToString();
    
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
    }

    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");

    // Responder imediatamente a requisições OPTIONS (preflight)
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }

    await next();
});
```

## ⚠️ Segurança

O código atual aceita qualquer origem por segurança. Para produção, considere restringir:

```csharp
var allowedOrigins = new List<string>
{
    "https://frontendtfc.vercel.app"
};

if (allowedOrigins.Contains(origin))
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
}
```

