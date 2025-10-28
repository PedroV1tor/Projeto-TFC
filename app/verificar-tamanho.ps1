Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verificando Tamanho dos Arquivos para Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar tamanho total
$totalSize = 0
Get-ChildItem -Recurse -File | ForEach-Object { $totalSize += $_.Length }
$totalSizeMB = [math]::Round($totalSize / 1MB, 2)

Write-Host "Tamanho total da pasta app: $totalSizeMB MB" -ForegroundColor Yellow
Write-Host ""

# Verificar node_modules
if (Test-Path "node_modules") {
    $nodeModulesSize = (Get-ChildItem "node_modules" -Recurse -File | Measure-Object Length -Sum).Sum
    $nodeModulesSizeMB = [math]::Round($nodeModulesSize / 1MB, 2)
    Write-Host "node_modules: $nodeModulesSizeMB MB (sera ignorado)" -ForegroundColor Red
}

# Verificar .angular
if (Test-Path ".angular") {
    $angularSize = (Get-ChildItem ".angular" -Recurse -File | Measure-Object Length -Sum).Sum
    $angularSizeMB = [math]::Round($angularSize / 1MB, 2)
    Write-Host ".angular: $angularSizeMB MB (sera ignorado)" -ForegroundColor Red
}

# Verificar dist
if (Test-Path "dist") {
    $distSize = (Get-ChildItem "dist" -Recurse -File | Measure-Object Length -Sum).Sum
    $distSizeMB = [math]::Round($distSize / 1MB, 2)
    Write-Host "dist: $distSizeMB MB (sera ignorado)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Tamanho que sera enviado (sem node_modules, .angular e dist):" -ForegroundColor Green
$totalSizeMB = [math]::Round(($totalSize - (if (Test-Path "node_modules") { $nodeModulesSize } else { 0 }) - (if (Test-Path ".angular") { $angularSize } else { 0 }) - (if (Test-Path "dist") { $distSize } else { 0 })) / 1MB, 2)
Write-Host "$totalSizeMB MB" -ForegroundColor Green

if ($totalSizeMB -gt 100) {
    Write-Host ""
    Write-Host "ATENCAO: Tamanho excede 100MB! Considerar usar --force ou verificar .vercelignore" -ForegroundColor Red
}

