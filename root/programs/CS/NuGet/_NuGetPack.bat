setlocal
@echo off
chcp 65001 >nul

@rem --------------------------------------------------
@rem バージョン番号を Directory.Build.props から取得する。
@rem （nuspec 側は $version$ トークンのままにしてあり、
@rem 　バージョンの定義箇所を1か所に集約している。）
@rem --------------------------------------------------
set OT_PROPS=..\Frameworks\Infrastructure\Directory.Build.props
set OT_VERSION=

for /f "usebackq delims=" %%v in (
  `powershell -NoProfile -ExecutionPolicy Bypass -Command "([xml](Get-Content '%OT_PROPS%' -Raw)).Project.PropertyGroup.OpenTouryoVersion"`
) do set OT_VERSION=%%v

if not defined OT_VERSION (
  echo [ERROR] %OT_PROPS% から OpenTouryoVersion を取得できませんでした。
  pause
  exit /b 1
)

echo --------------------------------------------------
echo OpenTouryoVersion = %OT_VERSION%
echo --------------------------------------------------

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

"..\..\nuget.exe" pack Symbol_Public.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Public.Security.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.RichClient.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamManagedOdp.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamPstGrS.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamMySQL.nuspec -Properties version=%OT_VERSION% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause
