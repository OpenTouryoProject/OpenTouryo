setlocal
@echo off

@rem --------------------------------------------------
@rem Read the version number from Directory.Build.props.
@rem The nuspec files keep the $version$ token, so the version is
@rem defined in exactly one place.
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem --------------------------------------------------
set OT_PROPS=..\Frameworks\Infrastructure\Directory.Build.props
set OT_VERSION=

for /f "usebackq delims=" %%v in (
  `powershell -NoProfile -ExecutionPolicy Bypass -Command "([xml](Get-Content '%OT_PROPS%' -Raw)).Project.PropertyGroup.OpenTouryoVersion"`
) do set OT_VERSION=%%v

if not defined OT_VERSION (
  echo [ERROR] Failed to read OpenTouryoVersion from %OT_PROPS%.
  pause
  exit /b 1
)

@rem --------------------------------------------------
@rem Read the commit hash (#531).
@rem It is passed to <repository commit="$commit$"> in the nuspec so that
@rem nuget.org shows which commit the package was built from.
@rem
@rem Source Link itself works from the information inside the PDB, so
@rem source debugging still works when this is empty. It only helps
@rem people navigate back to the source.
@rem --------------------------------------------------
set OT_COMMIT=

for /f "usebackq delims=" %%c in (`git rev-parse HEAD 2^>nul`) do set OT_COMMIT=%%c

if not defined OT_COMMIT (
  echo [WARN] Could not read the commit hash from git. Continuing with an empty value.
  set OT_COMMIT=
)

echo --------------------------------------------------
echo OpenTouryoVersion = %OT_VERSION%
echo commit            = %OT_COMMIT%
echo --------------------------------------------------

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

"..\..\nuget.exe" pack Symbol_Public.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Public.Security.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.RichClient.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamManagedOdp.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamPstGrS.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamMySQL.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause