setlocal
@echo off

"..\..\..\..\nuget.exe" SetApiKey [ApiKey]

"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.symbols.nupkg -source https://nuget.smbsrc.net/
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.symbols.nupkg -source https://nuget.smbsrc.net/

pause