# Gerenciamento de Publicações para Administradores

## 📋 Visão Geral

Foi implementada uma funcionalidade que permite aos usuários administradores gerenciar todas as publicações do sistema através da página "Laboratório".

## ✨ Funcionalidades Implementadas

### Backend

1. **Campo IsAdmin no Modelo Usuario**
   - Adicionado campo `IsAdmin` (boolean) ao modelo `Usuario`
   - Migration criada: `20251015193732_AdicionarCampoIsAdmin`

2. **Atualização do JWT Token**
   - O token JWT agora inclui a claim `isAdmin`
   - Administradores também recebem a role `Admin` no token

3. **LoginResponse Atualizado**
   - O DTO de resposta de login agora inclui o campo `IsAdmin`

### Frontend

1. **Atualização do AuthService**
   - Interface `Usuario` agora inclui `isAdmin`
   - Interface `LoginResponse` agora inclui `isAdmin`
   - Novo getter `isAdmin` para verificar se o usuário atual é admin
   - O status de admin é persistido no localStorage

2. **Atualização do AgendamentoService**
   - Novo método `getTodosAgendamentos()` que retorna todos os agendamentos do sistema
   - Método `getAgendamentos()` continua retornando apenas os agendamentos do usuário

3. **Componente Laboratório Atualizado**
   - Para usuários normais: exibe apenas seus agendamentos com botões de editar/excluir
   - Para administradores: 
     - Exibe **TODOS** os agendamentos do sistema
     - Botões para **alterar status** dos agendamentos (Aprovado, Pendente, Reprovado)
     - Botões de editar e excluir agendamentos
     - Exibe seção de gerenciamento de publicações

4. **Funcionalidades de Gerenciamento de Agendamentos (Admin)**
   - Visualizar todos os agendamentos do sistema
   - Alterar status dos agendamentos entre:
     - ✓ **Aprovado**: agendamento aprovado
     - ⏳ **Pendente**: aguardando aprovação
     - ✗ **Reprovado**: agendamento rejeitado
   - Editar e excluir agendamentos

5. **Funcionalidades de Gerenciamento de Publicações (Admin)**
   - Visualizar todas as publicações do sistema
   - Ver informações: título, resumo, autor, data de criação, visualizações
   - Alterar status das publicações entre:
     - ✓ **Ativa**: publicação visível para todos
     - ✎ **Rascunho**: publicação em edição
     - 📦 **Arquivada**: publicação arquivada
   - Excluir publicações

## 🚀 Como Testar

### 1. Atualizar o Banco de Dados

Execute a migration para adicionar o campo `IsAdmin`:

```bash
cd backend
dotnet ef database update
```

### 2. Configurar Usuário Admin

Um usuário admin já está configurado no seed data:

- **Email**: `admin@inovalab.com`
- **Senha**: `123456`
- **IsAdmin**: `true`

Se o banco de dados já foi criado antes desta atualização, você precisará:

**Opção A - Recriar o banco** (CUIDADO: apaga todos os dados):
```bash
cd backend
dotnet ef database drop
dotnet ef database update
```

**Opção B - Atualizar manualmente via SQL**:
```sql
-- Marcar o usuário admin como administrador
UPDATE "Usuarios" SET "IsAdmin" = true WHERE "Email" = 'admin@inovalab.com';
```

### 3. Iniciar o Backend

```bash
cd backend
dotnet run
```

### 4. Iniciar o Frontend

```bash
cd app
npm start
```

### 5. Testar a Funcionalidade

1. **Fazer login como admin**:
   - Acesse a página de login
   - Email: `admin@inovalab.com`
   - Senha: `123456`

2. **Navegar para a página Laboratório**:
   - Clique no menu "Laboratório"
   - Você verá duas seções:
     - 📅 **Agendamentos** (com botões de status para admin)
     - 📰 **Gerenciamento de Publicações** (seção exclusiva para admin)

3. **Gerenciar Agendamentos**:
   - Nos cards de agendamento, você verá uma nova seção "Alterar Status"
   - Clique nos botões de status para alterar:
     - **✓ Aprovado**: marca como aprovado (verde)
     - **⏳ Pendente**: marca como pendente (azul)
     - **✗ Reprovado**: marca como reprovado (vermelho)
   - Os botões são desabilitados quando o agendamento já está no status selecionado
   - Os botões de editar e excluir continuam disponíveis

4. **Gerenciar Publicações**:
   - Na seção de publicações, você verá todas as publicações do sistema
   - Clique nos botões de status para alterar:
     - **Ativa**: torna a publicação visível
     - **Rascunho**: marca como em edição
     - **Arquivada**: arquiva a publicação
   - Clique em "Excluir Publicação" para remover permanentemente

5. **Testar com usuário não-admin**:
   - Faça logout
   - Faça login com um usuário normal (ex: `joao@email.com`, senha: `123456`)
   - Navegue para "Laboratório"
   - Confirme que:
     - Vê apenas seus próprios agendamentos
     - NÃO vê os botões de alteração de status
     - NÃO vê a seção de publicações

## 🎨 Interface

### Seção de Agendamentos (Admin)

Cada card de agendamento contém:
- **Título e status** (badge colorido no topo)
- **Data** do agendamento
- **Usuário** que criou o agendamento
- **Descrição** do agendamento
- **Seção "Alterar Status"** (apenas para admin) com botões:
  - Verde: ✓ Aprovado
  - Azul: ⏳ Pendente
  - Vermelho: ✗ Reprovado
- **Botões de ação**: Editar e Excluir

### Seção de Publicações (Admin)

A seção de publicações aparece após a seção de agendamentos, com:

- **Título da seção**: "📰 Gerenciamento de Publicações"
- **Contador**: mostra o número total de publicações
- **Cards de publicação** contendo:
  - Título e status (badge colorido)
  - Data de criação
  - Autor
  - Número de visualizações
  - Resumo da publicação
  - Botões para alterar status:
    - Verde: Ativa
    - Amarelo: Rascunho
    - Cinza: Arquivada
  - Botão vermelho para excluir

### Estilos dos Status

**Agendamentos:**
- **Aprovado**: Verde (#d4edda)
- **Pendente**: Azul (#cce5ff)
- **Reprovado**: Vermelho (#f8d7da)

**Publicações:**
- **Ativa**: Verde (#d4edda)
- **Rascunho**: Amarelo (#fff3cd)
- **Arquivada**: Cinza (#e2e3e5)

## 🔒 Segurança

- Apenas usuários com `IsAdmin = true` podem ver a seção de gerenciamento
- O campo `IsAdmin` está no token JWT e é validado no frontend
- As rotas de API para alterar status e excluir publicações já possuem autenticação via JWT

## 📝 Arquivos Modificados

### Backend
- `backend/Models/Usuario.cs` - Adicionado campo IsAdmin
- `backend/Models/DTOs/LoginResponse.cs` - Adicionado campo IsAdmin
- `backend/Services/AuthService.cs` - Atualizado para incluir IsAdmin no token JWT
- `backend/Data/SeedData.cs` - Usuário admin marcado como IsAdmin = true
- `backend/Migrations/20251015193732_AdicionarCampoIsAdmin.cs` - Nova migration

### Frontend
- `app/src/app/services/auth.service.ts` - Adicionado suporte a IsAdmin
- `app/src/app/services/agendamento.service.ts` - Adicionado método getTodosAgendamentos()
- `app/src/app/components/laboratorio/laboratorio.component.ts` - Lógica de gerenciamento
- `app/src/app/components/laboratorio/laboratorio.component.html` - Interface de gerenciamento
- `app/src/app/components/laboratorio/laboratorio.component.scss` - Estilos para publicações

## 🐛 Troubleshooting

### A seção de publicações não aparece para o admin

1. Verifique se o usuário está realmente marcado como admin no banco:
   ```sql
   SELECT "Email", "IsAdmin" FROM "Usuarios" WHERE "Email" = 'admin@inovalab.com';
   ```

2. Verifique se o token JWT contém a claim `isAdmin`:
   - Abra o console do navegador
   - Vá para Application > Local Storage
   - Verifique o valor de `currentUser`
   - Deve conter `"isAdmin": true`

3. Se necessário, faça logout e login novamente para obter um novo token

### Os agendamentos não aparecem para o admin

1. Verifique no console do navegador se há mensagens de erro
2. Deve aparecer uma mensagem: `Carregados X agendamento(s) (admin - todos)`
3. Verifique se o backend está retornando dados no endpoint `/api/agendamento` (sem /meus)
4. Se necessário, crie alguns agendamentos de teste com usuários diferentes

### Erros ao alterar status

- Verifique se o backend está rodando
- Verifique se o token JWT é válido
- Verifique os logs do backend para mensagens de erro

## 📚 Referências

- [JWT Claims](https://jwt.io/)
- [Angular Authentication](https://angular.io/guide/security)
- [ASP.NET Core Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction)

