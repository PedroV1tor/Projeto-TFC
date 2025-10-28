@echo off
echo ============================================
echo  ANALISE DE SEGURANCA - InovalabAPI
echo ============================================
echo.

echo [1/3] Verificando pacotes vulneráveis...
echo.
dotnet list package --vulnerable --include-transitive

echo.
echo ============================================
echo.
echo [2/3] Verificando formatação de código...
echo.
dotnet format --verify-no-changes

echo.
echo ============================================
echo.
echo [3/3] Verificando pacotes desatualizados...
echo.
dotnet list package --outdated --include-transitive

echo.
echo ============================================
echo  ANALISE CONCLUIDA
echo ============================================
pause

