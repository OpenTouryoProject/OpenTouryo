setlocal
@echo off

"..\..\..\..\nuget.exe" SetApiKey [ApiKey]

"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.nupkg -source https://api.nuget.org/v3/index.json
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.nupkg -source https://api.nuget.org/v3/index.json

pause