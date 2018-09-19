setlocal
@echo off

"..\..\..\..\nuget.exe" SetApiKey [ApiKey]

"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.nupkg -source https://www.nuget.org/
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.nupkg -source https://www.nuget.org/

pause