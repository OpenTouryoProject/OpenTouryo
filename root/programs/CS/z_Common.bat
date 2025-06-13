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

echo BUILDFILEPATH2.0 %BUILDFILEPATH2.0%
echo BUILDFILEPATH3.5 %BUILDFILEPATH3.5%
echo BUILDFILEPATH4.0 %BUILDFILEPATH4.0%
echo BUILDFILEPATH15 %BUILDFILEPATH15%
echo BUILDFILEPATH16 %BUILDFILEPATH16%
echo BUILDFILEPATH17 %BUILDFILEPATH17%

set BUILDFILEPATH=%BUILDFILEPATH17%
echo BUILDFILEPATH %BUILDFILEPATH%

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo --------------------------------------------------
set BUILD_CONFIG=Debug
set VisualStudioVersion=17.0

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% -v:d

@echo --------------------------------------------------
@echo Set the proxy settings of Nuget.
@echo --------------------------------------------------
@rem set http_proxy=http://[username]:[password]@[proxy fqdn or ip address]