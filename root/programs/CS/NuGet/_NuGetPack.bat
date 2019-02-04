setlocal
@echo off

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net45" "in\net452"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_net46" "in\net46"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_net47" "in\net47"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netstd20\netstandard2.0" "in\netstandard2.0"

"..\..\nuget.exe" pack Public.nuspec -OutputDirectory "out\pp"
"..\..\nuget.exe" pack Public.Security.nuspec -OutputDirectory "out\pp"
"..\..\nuget.exe" pack Framework.nuspec -OutputDirectory "out\pp"
"..\..\nuget.exe" pack Framework.RichClient.nuspec -OutputDirectory "out\pp"
"..\..\nuget.exe" pack DamManagedOdp.nuspec -OutputDirectory "out\pp"
"..\..\nuget.exe" pack DamPstGrS.nuspec -OutputDirectory "out\pp"
"..\..\nuget.exe" pack DamMySQL.nuspec -OutputDirectory "out\pp"

"..\..\nuget.exe" pack Symbol_Public.nuspec -OutputDirectory "out\sp" -Symbols
"..\..\nuget.exe" pack Symbol_Public.Security.nuspec -OutputDirectory "out\sp" -Symbols
"..\..\nuget.exe" pack Symbol_Framework.nuspec -OutputDirectory "out\sp" -Symbols
"..\..\nuget.exe" pack Symbol_Framework.RichClient.nuspec -OutputDirectory "out\sp" -Symbols
"..\..\nuget.exe" pack Symbol_DamManagedOdp.nuspec -OutputDirectory "out\sp" -Symbols
"..\..\nuget.exe" pack Symbol_DamPstGrS.nuspec -OutputDirectory "out\sp" -Symbols
"..\..\nuget.exe" pack Symbol_DamMySQL.nuspec -OutputDirectory "out\sp" -Symbols

pause
