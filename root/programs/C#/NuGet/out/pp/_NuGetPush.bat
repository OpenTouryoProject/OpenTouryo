setlocal
@echo off

@rem ApiKeyを登録
..\..\..\..\nuget.exe SetApiKey [ApiKey]

@rem nuget.orgにPrimaryPackageを登録
..\..\..\..\nuget.exe push Touryo.Infrastructure.Public.*.nupkg -source https://www.nuget.org/
..\..\..\..\nuget.exe push Touryo.Infrastructure.Framework.*.nupkg -source https://www.nuget.org/

pause