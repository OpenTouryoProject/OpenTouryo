setlocal

@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Get the path to the executable file.
@rem --------------------------------------------------
set CURRENT_DIR="%~dp0"

@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------
call %CURRENT_DIR%z_Common.bat

rem --------------------------------------------------
rem Build the batch Infrastructure(Nuget47)
rem --------------------------------------------------
dotnet restore "Frameworks\Infrastructure\NuGet_netstd20.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\NuGet_netstd20.sln"

echo [32m32 "Microsoft/msbuild Issue #2221"
echo [32m32 "ResourceManager.GetString doesn't return embedded text file on linux."
echo [32m32 "https://github.com/Microsoft/msbuild/issues/2221"

pause

rem -------------------------------------------------------
endlocal
