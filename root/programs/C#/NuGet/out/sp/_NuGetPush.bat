setlocal
@echo off

@rem ApiKey��o�^
..\..\..\..\nuget.exe SetApiKey [ApiKey]

@rem symbolsource.org��SymbolPackage��o�^
..\..\..\..\nuget.exe push Touryo.Infrastructure.Public.*.symbols.nupkg -source https://nuget.smbsrc.net/
..\..\..\..\nuget.exe push Touryo.Infrastructure.Framework.*.symbols.nupkg -source https://nuget.smbsrc.net/

pause