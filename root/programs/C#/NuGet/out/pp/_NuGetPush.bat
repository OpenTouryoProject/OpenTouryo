setlocal
@echo off

@rem ApiKey��o�^
..\..\..\..\nuget.exe SetApiKey [ApiKey]

@rem nuget.org��PrimaryPackage��o�^
..\..\..\..\nuget.exe push Touryo.Infrastructure.Public.*.nupkg -source https://www.nuget.org/
..\..\..\..\nuget.exe push Touryo.Infrastructure.Framework.*.nupkg -source https://www.nuget.org/

pause