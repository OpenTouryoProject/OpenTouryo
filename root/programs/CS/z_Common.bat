@rem --------------------------------------------------
@rem Execution of the common processing.
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem
@rem cmd.exe reads a batch file by byte offset. Non-ASCII text is decoded
@rem with the console code page, which can misalign the parser and make it
@rem execute the tail of an @rem line as a command. A UTF-8 BOM reduces
@rem this but does not prevent it, and "chcp 65001" inside the file makes
@rem it worse by changing the code page half way through.
@rem
@rem This file is called by every build batch, so it is kept ASCII only.
@rem chcp is not used here; a caller that needs UTF-8 output sets it up.
@rem --------------------------------------------------

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
@rem Resolve MSBuild with vswhere, independently of the VS edition.
@rem The fixed paths above only look for Community, so this also finds
@rem Professional / Enterprise / BuildTools.
@rem --------------------------------------------------
set VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set BUILDFILEPATH=

if exist %VSWHERE% (
  for /f "usebackq tokens=*" %%i in (
    `%VSWHERE% -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`
  ) do set BUILDFILEPATH="%%i"
)

@rem --------------------------------------------------
@rem Fallback, for when vswhere is missing or resolves nothing.
@rem --------------------------------------------------
if not defined BUILDFILEPATH set BUILDFILEPATH=%BUILDFILEPATH18%
if not defined BUILDFILEPATH set BUILDFILEPATH=%BUILDFILEPATH17%
if not defined BUILDFILEPATH set BUILDFILEPATH=%BUILDFILEPATH16%

echo BUILDFILEPATH %BUILDFILEPATH%

@rem --------------------------------------------------
@rem Stop when nothing was found.
@rem Carrying on with an empty value makes "/p:Configuration=..." run as
@rem the command itself, which is hard to diagnose.
@rem --------------------------------------------------
if not defined BUILDFILEPATH (
  echo [ERROR] MSBuild.exe was not found.
  echo         Check the Visual Studio or Build Tools installation.
  exit /b 1
)

@rem --------------------------------------------------
@rem The MSBuild passed to nuget.exe restore.
@rem
@rem nuget.exe detects MSBuild on its own, but when another product that
@rem ships MSBuild is installed - SQL Server Management Studio, say - it
@rem can pick that one instead.
@rem   e.g. "Using MSBuild version ... from 'C:\Program Files\Microsoft SQL
@rem         Server Management Studio 22\Release\MSBuild\Current\bin'."
@rem
@rem That MSBuild has no Microsoft.WebApplication.targets for web apps, and
@rem the project.assets.json it writes does not match the real build, so
@rem errors like these follow.
@rem   error MSB4226: The imported project "...WebApplications\
@rem                  Microsoft.WebApplication.targets" was not found.
@rem   error : Your project file doesn't list 'win' as a "RuntimeIdentifier".
@rem
@rem The folder resolved above is therefore passed explicitly.
@rem --------------------------------------------------
for %%i in (%BUILDFILEPATH%) do set MSBUILDDIR=%%~dpi

@rem Strip the trailing backslash. In -MSBuildPath "...\" the \" would be
@rem read as an escape and the argument would break.
if defined MSBUILDDIR set MSBUILDDIR=%MSBUILDDIR:~0,-1%

set NUGET_MSBUILD=-MSBuildPath "%MSBUILDDIR%"

echo NUGET_MSBUILD %NUGET_MSBUILD%

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo BUILD_CONFIG names the configuration (Debug or Release).
@echo DEBUG_TYPE is full, pdbonly, portable, embedded or none.
@echo https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-options/code-generation#debugtype
@echo --------------------------------------------------
set BUILD_CONFIG=Debug

@rem --------------------------------------------------
@rem DEBUG_TYPE honours the value the caller has already set (#531).
@rem
@rem The build for the NuGet packages (0_Release4Nuget.bat) needs portable.
@rem   - a .snupkg is only accepted when the PDB is portable
@rem   - the Source Link information also lives in the portable PDB
@rem This line used to be edited by hand and reverted afterwards. Forgetting
@rem the revert meant publishing with "full", so the caller passes it now.
@rem --------------------------------------------------
if not defined DEBUG_TYPE set DEBUG_TYPE=full

@rem --------------------------------------------------
@rem CI_BUILD likewise honours the value the caller has already set (#531).
@rem
@rem true turns on ContinuousIntegrationBuild, which normalizes the source
@rem paths recorded in the PDB to /_/... .
@rem   - the published package no longer carries the local paths of the
@rem     machine that built it
@rem   - Visual Studio opens the file at the path in the PDB when it exists,
@rem     so an absolute path means Source Link is never used on the build
@rem     machine. Normalizing lets it be verified there as well.
@rem
@rem Off for ordinary builds. Only the build for the NuGet packages
@rem (0_Release4Nuget.bat) passes true.
@rem
@rem DeterministicSourcePaths is passed as well. The conversion from
@rem ContinuousIntegrationBuild is done by the Microsoft.NET.Sdk targets,
@rem so it does not reach the old-style projects - the same shape as Source
@rem Link not being picked up automatically. Without it, only the net48
@rem side keeps absolute paths.
@rem --------------------------------------------------
if not defined CI_BUILD set CI_BUILD=false

@rem --------------------------------------------------
@rem VisualStudioVersion comes from the same VS as the MSBuild resolved
@rem above.
@rem
@rem A web app csproj builds its targets path from this value.
@rem   <VSToolsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
@rem   <Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" />
@rem
@rem A fixed value breaks on a machine that only has another VS version.
@rem   e.g. windows-latest on GitHub Actions is VS 2022 = 17.x, so passing
@rem        18.0 makes it look for v18.0\WebApplications\ and fail MSB4226.
@rem --------------------------------------------------
set VSVER_MAJOR=

if exist %VSWHERE% (
  for /f "usebackq tokens=1 delims=." %%i in (
    `%VSWHERE% -latest -products * -requires Microsoft.Component.MSBuild -property installationVersion`
  ) do set VSVER_MAJOR=%%i
)

@rem Fall back to the previous value when vswhere is missing or resolves
@rem nothing.
if not defined VSVER_MAJOR set VSVER_MAJOR=18

set VisualStudioVersion=%VSVER_MAJOR%.0
echo VisualStudioVersion %VisualStudioVersion%

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
@set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% -v:d
set COMMANDLINE=/p:Configuration=%BUILD_CONFIG% /p:DebugType=%DEBUG_TYPE% /p:ContinuousIntegrationBuild=%CI_BUILD% /p:DeterministicSourcePaths=%CI_BUILD% -v:d

@echo --------------------------------------------------
@echo Set the proxy settings of Nuget.
@echo --------------------------------------------------
@rem set http_proxy=http://[username]:[password]@[proxy fqdn or ip address]