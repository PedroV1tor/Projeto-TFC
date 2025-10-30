@echo off
echo ========================================
echo  Deploy Backend C# na Vercel
echo ========================================
echo.

REM Verificar se o Vercel CLI está instalado
vercel --version >nul 2>&1
if errorlevel 1 (
    echo [ERRO] Vercel CLI nao encontrado!
    echo.
    echo Por favor, instale o Vercel CLI primeiro:
    echo npm install -g vercel
    echo.
    pause
    exit /b 1
)

echo Verificando se esta logado na Vercel...
vercel whoami >nul 2>&1
if errorlevel 1 (
    echo [INFO] Fazer login na Vercel...
    vercel login
)

echo.
echo IMPORTANTE: Certifique-se de configurar as variaveis de ambiente!
echo.
echo - ConnectionStrings__DefaultConnection
echo - JwtSettings__SecretKey
echo - EmailSettings__User
echo - EmailSettings__Password
echo - ProductionUrl (URL do frontend na Vercel)
echo.

set /p continua="Continuar com o deploy? (S/N): "
if /i not "%continua%"=="S" (
    echo Deploy cancelado.
    pause
    exit /b 1
)

echo.
echo Escolha uma opcao:
echo 1. Deploy para Preview (desenvolvimento)
echo 2. Deploy para Producao
echo.
set /p opcao="Digite o numero da opcao: "

if "%opcao%"=="1" (
    echo.
    echo Fazendo deploy para Preview...
    vercel
) else if "%opcao%"=="2" (
    echo.
    echo Fazendo deploy para Producao...
    vercel --prod
) else (
    echo Opcao invalida!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Deploy concluido!
echo ========================================
echo.
echo IMPORTANTE: Configure as variaveis de ambiente no painel da Vercel!
pause

