$json = '{"email":"admin@inovalab.com","senha":"123456"}'
$json | Out-File -FilePath "temp_login.json" -Encoding UTF8
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d "@temp_login.json"
Remove-Item "temp_login.json"
