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

if exist "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" (
  set BUILDFILEPATH18="C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe"
)

echo BUILDFILEPATH2.0 %BUILDFILEPATH2.0%
echo BUILDFILEPATH3.5 %BUILDFILEPATH3.5%
echo BUILDFILEPATH4.0 %BUILDFILEPATH4.0%
echo BUILDFILEPATH15 %BUILDFILEPATH15%
echo BUILDFILEPATH16 %BUILDFILEPATH16%
echo BUILDFILEPATH17 %BUILDFILEPATH17%
echo BUILDFILEPATH17 %BUILDFILEPATH18%

set BUILDFILEPATH=%BUILDFILEPATH18%
echo BUILDFILEPATH %BUILDFILEPATH%

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo BUILD_CONFIG は 特定の構成（Debug や Release）を指定
@echo DEBUG_TYPE は full, pdbonly, portable, embedded, none
@echo https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/compiler-options/code-generation#debugtype
@echo --------------------------------------------------
set BUILD_CONFIG=Debug
set DEBUG_TYPE=full
set VisualStudioVersion=18.0

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
@set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% -v:d
set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% /p:DebugType=%DEBUG_TYPE% -v:d

@echo --------------------------------------------------
@echo Set the proxy settings of Nuget.
@echo --------------------------------------------------
@rem set http_proxy=http://[username]:[password]@[proxy fqdn or ip address]