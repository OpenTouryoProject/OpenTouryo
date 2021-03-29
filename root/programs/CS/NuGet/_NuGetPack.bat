setlocal
@echo off

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net45" "in\net452"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_net46" "in\net46"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_net47" "in\net47"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netstd20\netstandard2.0" "in\netstandard2.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netstd21\netstandard2.1" "in\netstandard2.1"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore30\netcoreapp3.0" "in\netcoreapp3.0"

"..\..\nuget.exe" pack Symbol_Public.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Public.Security.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.RichClient.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamManagedOdp.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamPstGrS.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamMySQL.nuspec -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause
