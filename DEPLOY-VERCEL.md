# 🚀 Guia de Deploy na Vercel

## Pré-requisitos

1. Instalar a CLI da Vercel globalmente:
```bash
npm install -g vercel
```

2. Ter uma conta na Vercel (https://vercel.com)

## Passo a Passo para Deploy

### 1. Login na Vercel
```bash
vercel login
```

### 2. Configure as variáveis de ambiente (se necessário)

Antes de fazer o deploy, certifique-se de que a URL da API está correta no arquivo `app/src/environments/environment.prod.ts`.

### 3. Fazer o deploy

**IMPORTANTE**: Execute o deploy na pasta `app`, não na raiz do projeto!

```bash
cd app
vercel
```

Ou, se preferir, execute na raiz com:
```bash
vercel --cwd app
```

Na primeira vez, a Vercel vai fazer algumas perguntas:
- **Link to existing project?** → Escolha `N` para criar um novo projeto
- **What's your project's name?** → Digite um nome (ex: inovalab-frontend)
- **In which directory is your code located?** → Deixe vazio (./)
- **Want to override the settings?** → Escolha `N`

### 4. Deploy para produção

Após o deploy inicial, para fazer deploy para produção:

```bash
vercel --prod
```

## Comandos Disponíveis

### Preview Deploy
```bash
vercel
```

### Production Deploy
```bash
vercel --prod
```

### Ver informações do projeto
```bash
vercel inspect
```

### Ver logs em tempo real
```bash
vercel logs
```

### Listar deployments
```bash
vercel ls
```

## Estrutura de Arquivos

O arquivo `vercel.json` na pasta `app` está configurado para:
- **Build Command**: Executa `npm install && npm run build`
- **Output Directory**: `dist/app/browser`
- **Routes**: Configurado para SPA (Single Page Application) com fallback para index.html

**IMPORTANTE**: O deploy deve ser executado dentro da pasta `app`, não na raiz do projeto!

## Configuração do Ambiente de Produção

O arquivo `app/src/environments/environment.prod.ts` já está configurado com:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.inovalab.com/api'
};
```

**⚠️ IMPORTANTE**: Atualize a URL da API para a URL real do seu backend em produção.

## Troubleshooting

### Erro ao fazer build

Se o build falhar, verifique:
1. Se todas as dependências estão no `package.json`
2. Se o Node.js está na versão correta
3. Se o ambiente de produção está configurado corretamente

### Ajustar configurações

Para ajustar configurações de build, edite o arquivo `vercel.json` na raiz do projeto.

### Renovar deploy

Para forçar um novo deploy:
```bash
vercel --prod --force
```

## Verificar o Deploy

Após o deploy, você receberá uma URL do tipo:
- Preview: `https://seu-projeto-[hash].vercel.app`
- Production: `https://seu-projeto.vercel.app`

## Integração com Git (Opcional)

Para integração automática com Git:

1. Execute `vercel link` no projeto
2. Conecte seu repositório Git
3. Cada push automaticamente criará um novo deploy

```bash
vercel link
```

## Notas Importantes

- A Vercel detecta automaticamente o framework e ajusta o build
- O processo de build é executado na pasta `app`
- Todos os arquivos do Angular são servidos como arquivos estáticos
- As rotas do Angular são tratadas como SPA (todas as rotas apontam para index.html)

