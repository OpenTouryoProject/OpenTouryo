setlocal
@echo off

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

"..\..\nuget.exe" pack T_Symbol_Public.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
pause
