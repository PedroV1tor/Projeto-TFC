@echo off
echo ========================================
echo  Deploy Frontend Angular na Vercel
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

REM Entrar na pasta app
if not exist "app" (
    echo [ERRO] Pasta 'app' nao encontrada!
    echo Certifique-se de executar este script da raiz do projeto TFC-II.
    pause
    exit /b 1
)

echo.
echo Entrando na pasta app...
cd app

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
    cd ..
    pause
    exit /b 1
)

cd ..

echo.
echo ========================================
echo Deploy concluido!
echo ========================================
pause

