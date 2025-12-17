setlocal
@echo off

"..\..\..\..\nuget.exe" SetApiKey [ApiKey]

rem .nupkg と .snupkg の両方が在れば、両方が公開される。
rem -sourceオプションは、NuGet.Config の DefaultPushSource 値がなければ必須
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.nupkg -source https://api.nuget.org/v3/index.json
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.nupkg -source https://api.nuget.org/v3/index.json

rem 従って、以下のコマンドは実行不要になった。
rem "..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.snupkg -source https://api.nuget.org/v3/index.json
rem "..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.snupkg -source https://api.nuget.org/v3/index.json

pause