setlocal
@echo off

@rem --------------------------------------------------
@rem Build the test package (Erutcurtsarfni.Oyruot.Public).
@rem It is used to verify the symbol server and Source Link (#531).
@rem
@rem The version is passed as an argument. It is INDEPENDENT of the
@rem production version in Directory.Build.props.
@rem   e.g. _T_NuGetPack.bat 3.3.0-alpha1
@rem
@rem nuget.org only accepts a version higher than the published ones.
@rem   https://www.nuget.org/packages/Erutcurtsarfni.Oyruot.Public/
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem --------------------------------------------------
set OT_VERSION=%~1

if not defined OT_VERSION (
  echo [ERROR] Specify the package version.
  echo         e.g. _T_NuGetPack.bat 3.3.0-alpha1
  pause
  exit /b 1
)

@rem --------------------------------------------------
@rem Read the commit hash (#531).
@rem It is passed to <repository commit="$commit$"> in the nuspec.
@rem --------------------------------------------------
set OT_COMMIT=

for /f "usebackq delims=" %%c in (`git rev-parse HEAD 2^>nul`) do set OT_COMMIT=%%c

if not defined OT_COMMIT (
  echo [WARN] Could not read the commit hash from git. Continuing with an empty value.
  set OT_COMMIT=
)

echo --------------------------------------------------
echo version = %OT_VERSION%
echo commit  = %OT_COMMIT%
echo --------------------------------------------------

@rem --------------------------------------------------
@rem Clear the working folders before packing (#531).
@rem
@rem Only the test packages are removed. The production packages built by
@rem _NuGetPack.bat are left alone; their push uses a different wildcard.
@rem The reasons are in _Cleanup.bat, which can also be run on its own.
@rem --------------------------------------------------
call "%~dp0_Cleanup.bat" Erutcurtsarfni.Oyruot.Public /NOPAUSE

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

"..\..\nuget.exe" pack T_Symbol_Public.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause