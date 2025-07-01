setlocal
@echo off

"..\..\..\..\nuget.exe" SetApiKey [ApiKey]

"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.nupkg -source https://api.nuget.org/v3/index.json
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.nupkg -source https://api.nuget.org/v3/index.json

rem 以下でPushは出来たが、portableでは無いと怒られる。
rem ReleaseのDebugTypeはportableになっているので、
rem 次回はz_Common.batをReleaseに設定してPushしてみる。

"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.snupkg -source https://api.nuget.org/v3/index.json
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.snupkg -source https://api.nuget.org/v3/index.json

pause