setlocal
@echo off

@rem ApiKeyを登録
..\..\..\..\nuget.exe SetApiKey [ApiKey]

@rem symbolsource.orgにSymbolPackageを登録
..\..\..\..\nuget.exe push Touryo.Infrastructure.Public.*.symbols.nupkg -source https://nuget.smbsrc.net/
..\..\..\..\nuget.exe push Touryo.Infrastructure.Framework.*.symbols.nupkg -source https://nuget.smbsrc.net/

pause