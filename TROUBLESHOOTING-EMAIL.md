# 🔧 Troubleshooting - Problemas com Envio de Email em Produção

## ✅ Correções Aplicadas

1. **Serviço de Email corrigido**: Agora lança exceção em produção quando há erro
2. **Controller atualizado**: Trata erros de SMTP e retorna respostas adequadas
3. **Logs melhorados**: Melhor rastreamento de erros

## 🔍 Possíveis Causas de Falha no Envio

### 1. **Problema: Credenciais do Gmail Incorretas**

**Sintomas:**
- Erro: "The SMTP server requires a secure connection or the client was not authenticated"
- Status 500 no endpoint

**Solução:**
- Verifique se a senha no `appsettings.Production.json` está correta
- O Gmail pode exigir uma **Senha de App** (não a senha normal da conta)

**Como gerar Senha de App no Gmail:**
1. Acesse: https://myaccount.google.com/apppasswords
2. Selecione "App" → "Mail" e "Device" → "Other (Custom name)"
3. Digite um nome (ex: "Inovalab Production")
4. Clique em "Generate"
5. Use essa senha de 16 caracteres no `EmailSettings.Password`

### 2. **Problema: Gmail Bloqueando Conexões**

**Sintomas:**
- Erro: "SMTP authentication failed"
- Timeout na conexão

**Solução:**
- Verifique se "Acesso de app menos seguro" está desabilitado (já está desativado por padrão)
- Use uma **Senha de App** (solução acima)
- Verifique se a conta não está com restrições de segurança

### 3. **Problema: Firewall/Porta Bloqueada**

**Sintomas:**
- Erro: "Connection timeout" ou "Unable to connect to remote server"
- Timeout após 30 segundos

**Solução:**
- Verifique se a porta 587 está aberta no servidor Railway
- Verifique logs do Railway para erros de rede

### 4. **Problema: Configuração de Ambiente**

**Sintomas:**
- Email não envia mas não há erro
- Status 200 mas email não chega

**Solução:**
Verifique se as variáveis de ambiente estão configuradas no Railway:

```bash
ASPNETCORE_ENVIRONMENT=Production

EmailSettings__Host=smtp.gmail.com
EmailSettings__Port=587
EmailSettings__EnableSsl=true
EmailSettings__User=pedrovitormarques019@gmail.com
EmailSettings__Password=<senha-de-app>
EmailSettings__From=pedrovitormarques019@gmail.com
```

### 5. **Problema: Rate Limiting do Gmail**

**Sintomas:**
- Funciona algumas vezes, depois para
- Erro após muitos envios

**Solução:**
- Gmail tem limite de ~500 emails/dia para contas gratuitas
- Considere usar um serviço de email profissional (SendGrid, Mailgun, etc.)

## 🧪 Como Testar

### 1. Verificar Logs no Railway

Acesse os logs do serviço no Railway e procure por:
- `=== ENVIANDO EMAIL ===` (sucesso)
- `❌ Erro SMTP:` (erro)
- `✅ Email enviado com sucesso` (confirmação)

### 2. Testar Endpoint Manualmente

```bash
curl -X POST https://seu-backend.railway.app/api/auth/recuperar-senha \
  -H "Content-Type: application/json" \
  -d '{"email":"seu-email@exemplo.com"}'
```

**Respostas esperadas:**
- `200 OK`: Email enviado com sucesso
- `404 Not Found`: Email não encontrado
- `500 Internal Server Error`: Erro no envio (verifique logs)

### 3. Verificar Configurações

```bash
# Via Railway CLI
railway variables

# Ou no painel web do Railway
# Variables → Verifique EmailSettings
```

## 📝 Checklist de Verificação

Antes de considerar o problema resolvido, verifique:

- [ ] `ASPNETCORE_ENVIRONMENT=Production` está configurado
- [ ] Todas as variáveis `EmailSettings__*` estão configuradas no Railway
- [ ] A senha é uma **Senha de App** do Gmail (não a senha normal)
- [ ] A porta 587 não está bloqueada
- [ ] Os logs mostram tentativas de envio
- [ ] O email não está na caixa de spam
- [ ] A conta Gmail não está bloqueada/restrita

## 🔄 Próximos Passos se Ainda Não Funcionar

1. **Verificar logs detalhados**: Adicione mais logging no `SmtpEmailService`
2. **Testar com outro provedor**: Considere SendGrid ou Mailgun
3. **Verificar DNS/Conectividade**: Teste conexão SMTP manualmente
4. **Revisar configurações do Gmail**: Verifique segurança da conta

## 📞 Informações para Debug

Se ainda houver problemas, colete:
1. Logs completos do Railway (últimas 100 linhas)
2. Resposta exata do endpoint `/api/auth/recuperar-senha`
3. Status code e body da resposta
4. Timestamp do erro
5. Email de destino usado no teste

---

**Última atualização**: Após correções no `SmtpEmailService.cs` e `AuthController.cs`

