setlocal

@rem --------------------------------------------------
@rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
@rem       Japanese explanations are in Sample4SpRp\README.md.
@rem --------------------------------------------------

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
rem Batch build of the SpRp samples (Sample4SpRp, #649).
rem   All SpRp samples are built here (both net48 and net10.0).
rem   They need an external IdP (MPAS stub), so 2_RunAllTests / 3_SmokeTest
rem   do not run them. Check them by hand (Sample4SpRp\README.md).
rem --------------------------------------------------

rem --------------------------------------------------
rem SpRp_CLI (net48 and net10.0 from one csproj).
rem --------------------------------------------------
dotnet restore "Sample4SpRp\DotNet\CLI\SpRp_CLI.sln"
dotnet msbuild %COMMANDLINE% "Sample4SpRp\DotNet\CLI\SpRp_CLI.sln"

rem --------------------------------------------------
rem SpRp_MVC (net10.0 only for now, #668).
rem --------------------------------------------------
dotnet restore "Sample4SpRp\DotNet\MVC\SpRp_MVC.sln"
dotnet msbuild %COMMANDLINE% "Sample4SpRp\DotNet\MVC\SpRp_MVC.sln"

pause

rem -------------------------------------------------------
endlocal
