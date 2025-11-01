# 🚀 Guia Completo - Rodar Projeto em Desenvolvimento

Este guia explica passo a passo como executar o projeto TFC-II localmente em modo desenvolvimento.

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

1. **Node.js** (versão 18 ou superior)
   - Verificar: `node --version`
   - Download: https://nodejs.org/

2. **.NET 8.0 SDK**
   - Verificar: `dotnet --version`
   - Download: https://dotnet.microsoft.com/download

3. **Angular CLI** (instalado globalmente)
   - Instalar: `npm install -g @angular/cli`
   - Verificar: `ng version`

## 🎯 Passo a Passo

### 1️⃣ Instalar Dependências do Frontend (Angular)

Abra um terminal PowerShell ou CMD e execute:

```bash
cd "C:\Users\pedro vitor\Desktop\TFC-II\app"
npm install
```

Isso instalará todas as dependências do Angular listadas no `package.json`.

### 2️⃣ Instalar Dependências do Backend (.NET)

Em um novo terminal, execute:

```bash
cd "C:\Users\pedro vitor\Desktop\TFC-II\backend"
dotnet restore
```

Isso restaurará os pacotes NuGet necessários.

### 3️⃣ Configurar Ambiente de Desenvolvimento

O projeto já está configurado para desenvolvimento:

- **Frontend**: `app/src/environments/environment.ts` → `http://localhost:5000/api`
- **Backend**: `backend/Properties/launchSettings.json` → porta 5000

### 4️⃣ Iniciar o Backend

No terminal do backend, execute uma das opções:

**Opção A - Usando o script:**
```bash
cd "C:\Users\pedro vitor\Desktop\TFC-II\backend"
start.bat
```

**Opção B - Diretamente com dotnet:**
```bash
cd "C:\Users\pedro vitor\Desktop\TFC-II\backend"
dotnet run
```

**Verificação:**
- Backend deve iniciar em: `http://localhost:5000`
- Swagger UI disponível em: `http://localhost:5000/swagger` (apenas em dev)
- Você verá mensagens como: "Now listening on: http://localhost:5000"

### 5️⃣ Iniciar o Frontend

Em um novo terminal (mantenha o backend rodando), execute:

```bash
cd "C:\Users\pedro vitor\Desktop\TFC-II\app"
ng serve
```

Ou:

```bash
cd "C:\Users\pedro vitor\Desktop\TFC-II\app"
npm start
```

**Verificação:**
- Frontend deve iniciar em: `http://localhost:4200`
- Você verá: "Application bundle generation complete" e "Local: http://localhost:4200"

### 6️⃣ Acessar a Aplicação

Abra seu navegador e acesse:
```
http://localhost:4200
```

## 🔧 Troubleshooting (Solução de Problemas)

### ❌ Erro: "Status 0 - Unknown Error" ao fazer login

**Causa:** Backend não está rodando ou CORS não está configurado corretamente.

**Solução:**
1. Verifique se o backend está rodando na porta 5000
2. Acesse `http://localhost:5000/swagger` no navegador - deve abrir a documentação
3. Se não abrir, o backend não está rodando corretamente
4. Certifique-se de que o ambiente está configurado como `Development`

### ❌ Erro: "Cannot GET /" no frontend

**Causa:** Servidor Angular não iniciou corretamente.

**Solução:**
1. Pare o servidor (Ctrl+C)
2. Execute `npm install` novamente
3. Execute `ng serve` novamente

### ❌ Erro: "Port 5000 is already in use"

**Causa:** Outro processo está usando a porta 5000.

**Solução:**
1. Encontre o processo na porta 5000:
   ```powershell
   netstat -ano | findstr :5000
   ```
2. Termine o processo (substitua PID pelo número encontrado):
   ```powershell
   taskkill /PID <PID> /F
   ```
3. Ou altere a porta no `backend/Properties/launchSettings.json`

### ❌ Erro: "Port 4200 is already in use"

**Causa:** Outro processo Angular está rodando.

**Solução:**
```bash
# Pare o servidor atual (Ctrl+C) e inicie em outra porta:
ng serve --port 4201
```

Depois atualize o `app/src/environments/environment.ts` se necessário.

### ❌ Erro de CORS no console do navegador

**Causa:** Backend não está usando a política de CORS de desenvolvimento.

**Verificação:**
1. Confirme que `ASPNETCORE_ENVIRONMENT=Development` está configurado
2. No `Program.cs`, deve usar `app.UseCors("AllowDevelopment")` em desenvolvimento
3. Reinicie o backend após alterações no `Program.cs`

### ❌ Erro de conexão com banco de dados

**Causa:** String de conexão incorreta ou banco não acessível.

**Solução:**
1. Verifique `backend/appsettings.Development.json`
2. Confirme que a conexão PostgreSQL está correta
3. Teste a conexão com o banco de dados

## 📝 Estrutura de Portas

| Serviço | Porta | URL |
|---------|-------|-----|
| Frontend (Angular) | 4200 | http://localhost:4200 |
| Backend (API) | 5000 | http://localhost:5000 |
| Swagger (API Docs) | 5000 | http://localhost:5000/swagger |

## 🔍 Verificar se Tudo Está Funcionando

### Backend:
1. Acesse: `http://localhost:5000/swagger`
2. Deve mostrar a documentação da API
3. Teste o endpoint `GET /api/auth/test` (se existir)

### Frontend:
1. Acesse: `http://localhost:4200`
2. Deve carregar a página inicial
3. Tente fazer login

### Teste de Login:
1. Abra o console do navegador (F12)
2. Tente fazer login
3. Não deve aparecer erros de CORS
4. Deve receber resposta do backend (sucesso ou erro de credenciais)

## 🎯 Comandos Rápidos

### Terminal 1 - Backend:
```bash
cd backend
dotnet run
```

### Terminal 2 - Frontend:
```bash
cd app
ng serve
```

## 📌 Notas Importantes

1. **Sempre inicie o backend ANTES do frontend** para evitar erros de conexão
2. **Mantenha ambos os terminais abertos** enquanto desenvolve
3. **Alterações no backend** requerem reiniciar o servidor
4. **Alterações no frontend** são recarregadas automaticamente (hot-reload)
5. **CORS está configurado** para aceitar requisições de `http://localhost:4200`

## 🆘 Ainda com Problemas?

1. Verifique os logs no terminal do backend
2. Verifique o console do navegador (F12)
3. Confirme que as portas não estão bloqueadas pelo firewall
4. Certifique-se de que Node.js e .NET estão nas versões corretas

---

**Boa sorte com o desenvolvimento! 🚀**

