setlocal
@echo off

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

"..\..\nuget.exe" pack Symbol_Public.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Public.Security.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.RichClient.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamManagedOdp.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamPstGrS.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamMySQL.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause
