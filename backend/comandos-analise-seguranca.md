# 🔒 Comandos para Análise Estática de Segurança

## 📋 Guia Rápido

### Opção 1: Usando o Script (Mais Fácil) ✅

```bash
# Navegar para o diretório do backend
cd backend

# Executar o script de verificação
.\verificar-seguranca.bat
```

---

## 🔧 Comandos Individuais

### 1. Verificar Pacotes Vulneráveis
```bash
cd backend
dotnet list package --vulnerable --include-transitive
```

**Resultado:** Lista todos os pacotes com vulnerabilidades conhecidas (HIGH, MODERATE, LOW)

---

### 2. Verificar Pacotes Desatualizados
```bash
cd backend
dotnet list package --outdated --include-transitive
```

**Resultado:** Mostra pacotes que podem ser atualizados

---

### 3. Ver Formatação de Código
```bash
cd backend
dotnet format --verify-no-changes
```

**Resultado:** Verifica se há erros de formatação (sem corrigir)

---

### 4. Corrigir Formatação Automaticamente
```bash
cd backend
dotnet format
```

**Resultado:** Corrige automaticamente todos os erros de formatação

---

### 5. Ver Todas as Dependências
```bash
cd backend
dotnet list package --include-transitive
```

**Resultado:** Lista todos os pacotes e suas versões

---

## 🎯 Análise Completa em Um Comando

```bash
cd backend

# 1. Vulnerabilidades
dotnet list package --vulnerable --include-transitive

# 2. Formatação
dotnet format --verify-no-changes

# 3. Pacotes desatualizados
dotnet list package --outdated --include-transitive

# 4. Dependências transitivas
dotnet list package --include-transitive > dependencias.txt
```

---

## 📊 Interpretação dos Resultados

### Severidade HIGH 🔴
**Ação:** Atualizar urgentemente
- Risco crítico de segurança
- Pode ser explorado para comprometer o sistema

### Severidade MODERATE 🟡
**Ação:** Atualizar quando possível
- Risco moderado
- Geralmente não crítico imediato

### Severidade LOW 🟢
**Ação:** Atualizar na próxima revisão
- Risco baixo
- Não urgente

---

## 🛠️ Comandos Avançados

### Salvar Relatório em Arquivo
```bash
cd backend
dotnet list package --vulnerable --include-transitive > relatorio-vulnerabilidades.txt
```

### Exportar em JSON
```bash
cd backend
dotnet list package --vulnerable --include-transitive --json > relatorio.json
```

### Verificar Apenas Pacotes Diretos
```bash
cd backend
dotnet list package --vulnerable
```

---

## 📝 Exemplos de Saída

### Vulnerabilidades Identificadas:
```
Project `InovalabAPI` has the following vulnerable packages
   [net8.0]: 
   Transitive Package                         Resolved   Severity   Advisory URL
   > Microsoft.Extensions.Caching.Memory      8.0.0      High       https://github.com/advisories/GHSA-qj66-m88j-hmgj
   > System.Text.Json                         8.0.4      High       https://github.com/advisories/GHSA-8g4q-xg66-9fp4
```

### Formatação:
```
C:\path\to\file.cs(23,80): error WHITESPACE: Corrigir a formatação...
```

---

## ⚠️ Solucionando Vulnerabilidades

### Atualizar Pacote Específico
```bash
cd backend
dotnet add package Microsoft.Extensions.Caching.Memory --version 9.0.0
```

### Atualizar Todos os Pacotes
```bash
cd backend
dotnet add package --update-all
```

### Restaurar Dependências
```bash
cd backend
dotnet restore
```

---

## 🎯 Frequência Recomendada

- **Semanal:** Verificar vulnerabilidades
- **Mensal:** Revisar e atualizar pacotes
- **Antes de Deploy:** Executar análise completa
- **Após Atualizações:** Reexecutar todos os testes

---

## 📚 Recursos Úteis

- [NuGet Security Advisory](https://github.com/advisories?q=language%3Adotnet)
- [.NET Security](https://docs.microsoft.com/en-us/dotnet/core/security/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

