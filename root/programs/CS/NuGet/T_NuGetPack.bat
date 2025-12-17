setlocal
@echo off

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore80\net8.0" "in\net8.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore80\net8.0-windows" "in\net8.0-windows"

"..\..\nuget.exe" pack T_Symbol_Public.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
pause
