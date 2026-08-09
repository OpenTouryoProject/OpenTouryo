setlocal
@echo off
chcp 65001 >nul

@rem --------------------------------------------------
@rem テスト用パッケージ（Erutcurtsarfni.Oyruot.Public）を作る。
@rem シンボル サーバー・ソース サーバーの確認に使う（#531）。
@rem
@rem 版は引数で渡す。**本番版（Directory.Build.props）とは独立**に付ける。
@rem   例： _T_NuGetPack.bat 3.3.0-alpha1
@rem
@rem 公開済みより大きい版でなければ nuget.org は受け付けない。
@rem   https://www.nuget.org/packages/Erutcurtsarfni.Oyruot.Public/
@rem --------------------------------------------------
set OT_VERSION=%~1

if not defined OT_VERSION (
  echo [ERROR] 版を指定してください。
  echo         例: _T_NuGetPack.bat 3.3.0-alpha1
  pause
  exit /b 1
)

@rem --------------------------------------------------
@rem コミット ハッシュを取得する（#531）。
@rem nuspec の ^<repository commit="$commit$"^> に渡す。
@rem --------------------------------------------------
set OT_COMMIT=

for /f "usebackq delims=" %%c in (`git rev-parse HEAD 2^>nul`) do set OT_COMMIT=%%c

if not defined OT_COMMIT (
  echo [WARN] git からコミット ハッシュを取得できませんでした。空のまま続行します。
  set OT_COMMIT=
)

echo --------------------------------------------------
echo version = %OT_VERSION%
echo commit  = %OT_COMMIT%
echo --------------------------------------------------

xcopy /E /Y "..\Frameworks\Infrastructure\Build_net48" "in\net48"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "..\Frameworks\Infrastructure\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

"..\..\nuget.exe" pack T_Symbol_Public.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause
