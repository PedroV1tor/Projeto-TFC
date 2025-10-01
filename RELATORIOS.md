# Sistema de Relatórios - INOVALAB

## Visão Geral

O sistema de relatórios permite aos usuários gerar relatórios detalhados sobre diferentes aspectos do sistema, incluindo:

- **Relatório de Usuários**: Análise detalhada dos usuários cadastrados
- **Relatório de Publicações**: Estatísticas das publicações realizadas
- **Relatório de Agendamentos**: Visão geral dos agendamentos
- **Relatório de Orçamentos**: Análise dos orçamentos solicitados

## Funcionalidades Implementadas

### 1. Interface de Usuário
- Layout com 4 cards principais para seleção do tipo de relatório
- Formulário para seleção de período (data inicial e final)
- Botão de geração de relatório com validações
- Tabela responsiva para exibição dos dados
- Botões de exportação (PDF e Excel)

### 2. Componente Angular
- **Arquivo**: `app/src/app/components/relatorios/relatorios.component.ts`
- **Recursos**:
  - Seleção de tipo de relatório
  - Validação de datas
  - Geração de relatórios com filtros
  - Formatação de dados para exibição
  - Integração com serviço de relatórios

### 3. Serviço de Relatórios
- **Arquivo**: `app/src/app/services/relatorio.service.ts`
- **Recursos**:
  - Geração de relatórios por tipo e período
  - Dados mock para demonstração
  - Exportação para CSV (implementado)
  - Base para exportação PDF (preparado para implementação)

### 4. Estilos e Design
- **Arquivo**: `app/src/app/components/relatorios/relatorios.component.scss`
- **Características**:
  - Design responsivo
  - Cores consistentes com o sistema (#2B5CE6, #12bb20)
  - Animações e transições suaves
  - Cards com hover effects
  - Tabela estilizada

## Estrutura de Dados

### Relatório de Usuários
```typescript
{
  id: number;
  nome: string;
  email: string;
  telefone: string;
  criadoEm: string;
  status: string;
}
```

### Relatório de Publicações
```typescript
{
  id: number;
  titulo: string;
  autor: string;
  status: string;
  visualizacoes: number;
  criadoEm: string;
}
```

### Relatório de Agendamentos
```typescript
{
  id: number;
  titulo: string;
  data: string;
  usuario: string;
  status: string;
  criadoEm: string;
}
```

### Relatório de Orçamentos
```typescript
{
  id: number;
  titulo: string;
  cliente: string;
  valor: number;
  status: string;
  criadoEm: string;
}
```

## Como Usar

1. **Acesso**: Faça login no sistema e acesse "Serviços" > "Emitir Relatórios"
2. **Seleção**: Clique em um dos 4 cards para selecionar o tipo de relatório
3. **Período**: Defina a data inicial e final (padrão: últimos 30 dias)
4. **Geração**: Clique em "Gerar Relatório"
5. **Exportação**: Use os botões "Exportar PDF" ou "Exportar Excel"

## Integração com Backend

O serviço está preparado para integração com o backend. Para conectar com a API real:

1. Descomente as linhas no método `generateReport()` do `RelatorioService`
2. Implemente os endpoints correspondentes no backend:
   - `GET /api/relatorios/usuarios?dataInicial=&dataFinal=`
   - `GET /api/relatorios/publicacoes?dataInicial=&dataFinal=`
   - `GET /api/relatorios/agendamentos?dataInicial=&dataFinal=`
   - `GET /api/relatorios/orcamentos?dataInicial=&dataFinal=`

## Próximos Passos

1. **Backend**: Implementar endpoints de relatórios na API
2. **Exportação PDF**: Integrar biblioteca jsPDF
3. **Gráficos**: Adicionar visualizações gráficas (Chart.js)
4. **Filtros Avançados**: Implementar filtros adicionais por status, autor, etc.
5. **Agendamento**: Permitir agendamento de relatórios automáticos

## Dependências

- Angular 17+
- CommonModule
- FormsModule
- HttpClient (para integração futura com backend)

## Arquivos Criados/Modificados

### Novos Arquivos
- `app/src/app/components/relatorios/relatorios.component.html`
- `app/src/app/components/relatorios/relatorios.component.ts`
- `app/src/app/components/relatorios/relatorios.component.scss`
- `app/src/app/services/relatorio.service.ts`
- `app/src/environments/environment.ts`

### Arquivos Modificados
- `app/src/app/app.routes.ts` - Adicionada rota para relatórios
- `app/src/app/components/header/header.component.html` - Adicionado link no menu
- `app/src/app/components/header/header.component.ts` - Adicionado método de navegação

## Demonstração

O sistema inclui dados mock para demonstração imediata:
- 3 usuários de exemplo
- 3 publicações de exemplo
- 3 agendamentos de exemplo
- 3 orçamentos de exemplo

Todos os dados incluem formatação adequada e são filtráveis por período.
