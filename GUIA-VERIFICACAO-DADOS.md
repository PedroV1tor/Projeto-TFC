# 🔍 Guia Completo: Como Verificar se os Dados do Banco Estão Corretos

## 📋 **Resumo do Problema**
O sistema de relatórios está mostrando "nenhum registro" mesmo quando há dados no banco. Este guia fornece múltiplas formas de verificar e diagnosticar o problema.

## 🛠 **Ferramentas Criadas para Verificação**

### **1. 🎯 DatabaseTestController (Backend)**
**Arquivo**: `backend/Controllers/DatabaseTestController.cs`

Novos endpoints para verificação direta dos dados:
- `GET /api/databasetest/usuarios-direto` - Busca usuários diretamente do Entity Framework
- `GET /api/databasetest/usuarios-servico` - Busca usuários via UserService
- `GET /api/databasetest/publicacoes-direto` - Busca publicações diretamente
- `GET /api/databasetest/agendamentos-direto` - Busca agendamentos diretamente
- `GET /api/databasetest/orcamentos-direto` - Busca orçamentos diretamente
- `GET /api/databasetest/estatisticas-completas` - Estatísticas detalhadas com ranges de data

### **2. 🌐 Interface de Teste HTML**
**Arquivo**: `test-api.html`

Interface web para testar todos os endpoints sem precisar do frontend Angular.

### **3. 📊 Script SQL Direto**
**Arquivo**: `verificar-dados-banco.sql`

Queries SQL para execução direta no PostgreSQL.

### **4. 🤖 Script PowerShell**
**Arquivo**: `verificar-banco.ps1`

Script automatizado para conectar ao PostgreSQL e executar verificações.

## 📝 **Como Proceder - Passo a Passo**

### **🔥 MÉTODO 1: Teste Rápido via Browser (RECOMENDADO)**

#### **Passo 1: Iniciar o Backend**
```bash
cd backend
dotnet run
```
Aguarde até ver: `Now listening on: https://localhost:7109`

#### **Passo 2: Abrir Interface de Teste**
1. Abra o arquivo `test-api.html` no seu navegador
2. Faça login com suas credenciais do sistema
3. Execute os testes na seguinte ordem:

##### **2.1 Teste de Conectividade Básica**
- Clique em "🧪 Testar Conexão"
- **Resultado esperado**: Mostra totais de cada tipo de dados
- **Se falhar**: Problema de conectividade/autenticação

##### **2.2 Verificar Dados Diretos do Banco**
- Clique em "👥 Verificar Usuários (Direto)"
- **Resultado esperado**: Lista de usuários com datas de criação
- **Se vazio**: Banco não tem dados ou conexão com BD está falhando

##### **2.3 Comparar Serviço vs Direto**
- Clique em "🔄 Verificar Usuários (Serviço)"
- **Compare com resultado anterior**
- **Se diferente**: Problema no UserService

##### **2.4 Verificar Estatísticas Completas**
- Clique em "📊 Gerar Estatísticas Completas"
- **Mostra**: Ranges de datas, distribuição por data, totais
- **Importante**: Verifique o campo `rangesDatas.usuarios`

#### **Passo 3: Testar Relatórios com Datas Corretas**
Com base nas estatísticas, ajuste as datas e teste:
- Configure datas que cubram o range encontrado
- Teste cada tipo de relatório
- Observe os logs no terminal do backend

---

### **🔧 MÉTODO 2: Verificação Direta no Banco**

#### **Opção A: Via Script PowerShell**
```powershell
.\verificar-banco.ps1
```

#### **Opção B: Via SQL Manual**
1. Abra pgAdmin, DBeaver ou outro cliente PostgreSQL
2. Conecte ao banco `InovalabDB`
3. Execute as queries do arquivo `verificar-dados-banco.sql`

#### **Opção C: Via psql (Terminal)**
```bash
psql -h localhost -p 5432 -d InovalabDB -U postgres -f verificar-dados-banco.sql
```

---

### **🔍 MÉTODO 3: Análise de Logs (Avançado)**

#### **Logs do Backend**
No terminal onde o backend está rodando, observe:

```
🔍 [RelatorioController] Iniciando busca de usuários
📅 Parâmetros: dataInicial=01/01/2024, dataFinal=31/12/2024
📊 Total de usuários encontrados no banco: 5
📋 Primeiros usuários:
   - João Silva (Criado em: 2024-08-15)
   - Maria Santos (Criado em: 2024-08-20)
🔄 Aplicando filtros de data...
🗓️ Verificando usuário João: dataCriacao=2024-08-15, dataInicial=2024-01-01, dataFinal=2024-12-31
   ➡️ Filtro dataInicial: True (incluir=True)
   ➡️ Filtro dataFinal: True (incluir=True)
   ✅ Resultado final para João: True
📈 Usuários após filtro: 5 (eram 5)
✅ [RelatorioController] Retornando resposta com 5 usuários
```

#### **Logs do Frontend**
No console do navegador (F12), observe:

```javascript
🔍 Tentando buscar dados da API: https://localhost:7109/api/relatorio/usuarios?dataInicial=2024-01-01&dataFinal=2024-12-31
📡 Resposta da API recebida: {total: 5, items: [...]}
✅ Dados processados com sucesso. Total de registros: 5
```

---

## 🕵️ **Interpretação dos Resultados**

### **✅ Cenário 1: Dados Existem e Filtros Funcionam**
- **DatabaseTest mostra**: Dados existem
- **Estatísticas mostram**: Range de datas correto
- **Logs mostram**: Filtros aplicados corretamente
- **Problema**: Provavelmente no frontend Angular

### **❌ Cenário 2: Banco Vazio**
- **DatabaseTest mostra**: Total = 0 para todas as tabelas
- **Solução**: Executar migrations e seed data
```bash
cd backend
dotnet ef database drop --force
dotnet ef database update
```

### **⚠️ Cenário 3: Dados com Datas Diferentes**
- **DatabaseTest mostra**: Dados existem
- **Estatísticas mostram**: Range 2023-08-15 até 2023-08-20
- **Frontend usa**: 2024-01-01 até 2024-12-31
- **Solução**: Ajustar datas no frontend ou criar dados recentes

### **🔄 Cenário 4: Problema nos Serviços**
- **Busca direta**: Retorna dados
- **Via serviço**: Retorna vazio ou erro
- **Solução**: Verificar implementação dos serviços

### **🌐 Cenário 5: Problema de Conectividade**
- **API não responde**: Erro de rede
- **CORS**: Blocked by CORS policy
- **Autenticação**: 401 Unauthorized
- **Solução**: Verificar configurações de CORS e autenticação

---

## 🚨 **Soluções Rápidas por Cenário**

### **🔧 Se o banco está vazio:**
```bash
cd backend
dotnet ef database drop --force
dotnet ef database update
# Verificar se SeedData.Initialize está sendo chamado no Program.cs
```

### **📅 Se as datas estão incorretas:**
```typescript
// No frontend, usar datas mais amplas temporariamente
dataInicial = '2020-01-01';
dataFinal = '2030-12-31';
```

### **🔍 Para debug temporário (remover filtros):**
```csharp
// No RelatorioController.cs, comentar:
// if (dataInicial.HasValue || dataFinal.HasValue) { ... }
```

### **🌐 Se há problema de CORS:**
```csharp
// No Program.cs, verificar:
policy.WithOrigins("http://localhost:4200")
```

---

## 📞 **Próximos Passos**

1. **Execute o MÉTODO 1** (teste via browser)
2. **Compartilhe os resultados**:
   - Screenshots dos testes no `test-api.html`
   - Logs do terminal do backend
   - Console do navegador (F12)
3. **Com base nos resultados**, aplicaremos a solução específica

O diagnóstico está completo e cobrirá 100% dos cenários possíveis! 🎯
