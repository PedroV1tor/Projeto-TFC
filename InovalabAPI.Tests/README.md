# Testes Unitários - InovalabAPI

## Descrição

Este projeto contém testes unitários para validar as principais funcionalidades do backend da aplicação Inovalab.

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **Moq**: Library para criação de mocks
- **FluentAssertions**: Assertions mais legíveis
- **Entity Framework Core InMemory**: Banco de dados em memória para testes

## Estrutura dos Testes

### Services Testados

1. **AuthServiceTests** - Testes de autenticação e autorização
   - Login com credenciais válidas/inválidas
   - Registro de usuários
   - Recuperação de senha
   - Validação de tokens

2. **UserServiceTests** - Testes de gerenciamento de usuários
   - CRUD de usuários
   - Busca por ID e Email
   - Alteração de status
   - Atualização de dados

3. **PublicacaoServiceTests** - Testes de publicações
   - CRUD de publicações
   - Filtragem por status
   - Alteração de status

4. **AgendamentoServiceTests** - Testes de agendamentos
   - CRUD de agendamentos
   - Filtragem por status e período
   - Alteração de status

5. **OrcamentoServiceTests** - Testes de orçamentos
   - CRUD de orçamentos
   - Filtragem por status e valor
   - Alteração de status

## Como Executar os Testes

### Pré-requisitos

- .NET 8.0 SDK instalado
- **IMPORTANTE**: Certifique-se de que a API não está em execução antes de executar os testes

### Executar Todos os Testes

```bash
cd InovalabAPI.Tests
dotnet test
```

### Executar com Detalhes

```bash
dotnet test --verbosity detailed
```

### Executar com Cobertura de Código

```bash
dotnet test /p:CollectCoverage=true
```

### Executar Testes Específicos

```bash
# Executar apenas testes de AuthService
dotnet test --filter AuthServiceTests

# Executar apenas testes de UserService
dotnet test --filter UserServiceTests
```

## Cobertura de Testes

Os testes cobrem as seguintes funcionalidades principais:

### AuthService (8 testes)
- ✅ Login com credenciais válidas
- ✅ Login com senha inválida
- ✅ Login com email inexistente
- ✅ Login com usuário inativo
- ✅ Registro de usuário com dados válidos
- ✅ Registro com email duplicado
- ✅ Solicitação de recuperação de senha
- ✅ Recuperação de senha com email inexistente

### UserService (8 testes)
- ✅ Obter todos os usuários
- ✅ Obter usuário por ID
- ✅ Obter usuário por email
- ✅ Atualizar usuário
- ✅ Deletar usuário
- ✅ Alternar status do usuário

### PublicacaoService (8 testes)
- ✅ Criar publicação
- ✅ Obter todas as publicações
- ✅ Obter publicações por status
- ✅ Obter publicação por ID
- ✅ Atualizar publicação
- ✅ Deletar publicação
- ✅ Alterar status da publicação

### AgendamentoService (9 testes)
- ✅ Criar agendamento
- ✅ Obter todos os agendamentos
- ✅ Obter agendamentos por status
- ✅ Obter agendamento por ID
- ✅ Atualizar agendamento
- ✅ Deletar agendamento
- ✅ Alterar status do agendamento
- ✅ Obter agendamentos por período

### OrcamentoService (10 testes)
- ✅ Criar orçamento
- ✅ Obter todos os orçamentos
- ✅ Obter orçamentos por status
- ✅ Obter orçamento por ID
- ✅ Atualizar orçamento
- ✅ Deletar orçamento
- ✅ Alterar status do orçamento
- ✅ Obter orçamentos por valor mínimo

## Total: 43 Testes Unitários

## Padrões Utilizados

### Arrange-Act-Assert (AAA)

Todos os testes seguem o padrão AAA:

```csharp
[Fact]
public async Task MetodoTeste_Condicao_ResultadoEsperado()
{
    // Arrange - Preparar os dados de teste
    var dados = new Dados { ... };

    // Act - Executar a ação
    var resultado = await _service.Metodo(dados);

    // Assert - Verificar o resultado
    resultado.Should().NotBeNull();
    resultado.Propriedade.Should().Be("ValorEsperado");
}
```

### Nomenclatura de Testes

- **Método_Condição_ResultadoEsperado**
  - Exemplo: `Login_ComCredenciaisValidas_DeveRetornarToken`

### Banco de Dados em Memória

Cada teste utiliza uma instância isolada do banco de dados em memória:

```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

## Próximos Passos

- [ ] Adicionar testes de integração
- [ ] Adicionar testes de performance
- [ ] Configurar CI/CD para executar testes automaticamente
- [ ] Aumentar cobertura de código para 90%+
- [ ] Adicionar testes de segurança

## Contribuindo

Ao adicionar novas funcionalidades, sempre crie testes correspondentes seguindo os padrões estabelecidos neste projeto.

