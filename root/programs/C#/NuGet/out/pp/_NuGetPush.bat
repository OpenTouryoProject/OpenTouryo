setlocal
@echo off

@rem ApiKey‚ð“o˜^
..\..\..\..\nuget.exe SetApiKey [ApiKey]

@rem nuget.org‚ÉPrimaryPackage‚ð“o˜^
..\..\..\..\nuget.exe push Touryo.Infrastructure.Public.*.nupkg -source https://www.nuget.org/
..\..\..\..\nuget.exe push Touryo.Infrastructure.Framework.*.nupkg -source https://www.nuget.org/

pause