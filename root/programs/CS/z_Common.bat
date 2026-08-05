@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------

@rem --------------------------------------------------
@rem 文字化け対策
@rem --------------------------------------------------
chcp 65001

@rem --------------------------------------------------
@rem Specifying Build tool.
@rem --------------------------------------------------
set BUILDFILEPATH2.0="C:\Windows\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe"
set BUILDFILEPATH3.5="C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"
set BUILDFILEPATH4.0="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
  set BUILDFILEPATH15="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" (
  set BUILDFILEPATH15="C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe"
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH16="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH16="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH16="C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH17="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH17="C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
)
if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH17="C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

if exist "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH18="C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
)

echo BUILDFILEPATH2.0 %BUILDFILEPATH2.0%
echo BUILDFILEPATH3.5 %BUILDFILEPATH3.5%
echo BUILDFILEPATH4.0 %BUILDFILEPATH4.0%
echo BUILDFILEPATH15 %BUILDFILEPATH15%
echo BUILDFILEPATH16 %BUILDFILEPATH16%
echo BUILDFILEPATH17 %BUILDFILEPATH17%
echo BUILDFILEPATH18 %BUILDFILEPATH18%

@rem --------------------------------------------------
@rem vswhere による MSBuild の解決（エディション非依存）
@rem 上記の固定パスは Community しか探さないため、
@rem Professional / Enterprise / BuildTools でも解決できるようにする。
@rem --------------------------------------------------
set VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set BUILDFILEPATH=

if exist %VSWHERE% (
  for /f "usebackq tokens=*" %%i in (
    `%VSWHERE% -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`
  ) do set BUILDFILEPATH="%%i"
)

@rem --------------------------------------------------
@rem フォールバック（vswhere が無い / 解決できない場合）
@rem --------------------------------------------------
if not defined BUILDFILEPATH set BUILDFILEPATH=%BUILDFILEPATH18%
if not defined BUILDFILEPATH set BUILDFILEPATH=%BUILDFILEPATH17%
if not defined BUILDFILEPATH set BUILDFILEPATH=%BUILDFILEPATH16%

echo BUILDFILEPATH %BUILDFILEPATH%

@rem --------------------------------------------------
@rem 未検出チェック
@rem （空のまま進むと "/p:Configuration=..." が先頭コマンドとして
@rem 　実行され、原因が分かりにくいエラーになるため、ここで止める。）
@rem --------------------------------------------------
if not defined BUILDFILEPATH (
  echo [ERROR] MSBuild.exe が見つかりません。
  echo         Visual Studio または Build Tools のインストール状況を確認してください。
  exit /b 1
)

@rem --------------------------------------------------
@rem nuget.exe restore に渡す MSBuild の指定
@rem
@rem nuget.exe は MSBuild を自動検出するが、SQL Server Management Studio など
@rem MSBuild を同梱する別製品が入っていると、そちらを選ぶことがある。
@rem （例: "MSBuild 自動検出: 'C:\Program Files\Microsoft SQL Server Management
@rem 　Studio 22\Release\MSBuild\Current\bin' から MSBuild バージョン ... を使用します。"）
@rem
@rem その MSBuild には Web アプリ用の Microsoft.WebApplication.targets が無く、
@rem また生成される project.assets.json が実際のビルドと噛み合わないため、
@rem 下記のようなエラーになる。
@rem   error MSB4226: インポートされたプロジェクト "...WebApplications\
@rem                  Microsoft.WebApplication.targets" が見つかりませんでした。
@rem   error : Your project file doesn't list 'win' as a "RuntimeIdentifier".
@rem
@rem このため、上で解決した MSBuild のフォルダを明示的に渡す。
@rem --------------------------------------------------
for %%i in (%BUILDFILEPATH%) do set MSBUILDDIR=%%~dpi

@rem 末尾の \ を除去する（-MSBuildPath "...\" は \" が
@rem エスケープと解釈され、引数が壊れるため）。
if defined MSBUILDDIR set MSBUILDDIR=%MSBUILDDIR:~0,-1%

set NUGET_MSBUILD=-MSBuildPath "%MSBUILDDIR%"

echo NUGET_MSBUILD %NUGET_MSBUILD%

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo BUILD_CONFIG は 特定の構成（Debug や Release）を指定
@echo DEBUG_TYPE は full, pdbonly, portable, embedded, none
@echo https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/compiler-options/code-generation#debugtype
@echo --------------------------------------------------
set BUILD_CONFIG=Debug
set DEBUG_TYPE=full

@rem --------------------------------------------------
@rem VisualStudioVersion は、上で解決した MSBuild と同じ VS から求める。
@rem
@rem Web アプリの csproj は、この値で targets のパスを組み立てる。
@rem   <VSToolsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
@rem   <Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" />
@rem
@rem 固定値にすると、別のバージョンの VS しか無い環境で見つからなくなる。
@rem （例: GitHub Actions の windows-latest は VS 2022 = 17.x のため、
@rem 　18.0 を渡すと v18.0\WebApplications\ を探して MSB4226 になる）
@rem --------------------------------------------------
set VSVER_MAJOR=

if exist %VSWHERE% (
  for /f "usebackq tokens=1 delims=." %%i in (
    `%VSWHERE% -latest -products * -requires Microsoft.Component.MSBuild -property installationVersion`
  ) do set VSVER_MAJOR=%%i
)

@rem vswhere が無い / 解決できない場合は従来の値にする。
if not defined VSVER_MAJOR set VSVER_MAJOR=18

set VisualStudioVersion=%VSVER_MAJOR%.0
echo VisualStudioVersion %VisualStudioVersion%

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
@set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% -v:d
set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% /p:DebugType=%DEBUG_TYPE% -v:d

@echo --------------------------------------------------
@echo Set the proxy settings of Nuget.
@echo --------------------------------------------------
@rem set http_proxy=http://[username]:[password]@[proxy fqdn or ip address]