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

@rem --------------------------------------------------
@rem NUnit tests (#610).
@rem   Each csproj targets both net48 and net10.0.
@rem   The results (*.trx) are judged by 2_RunAllTests.ps1.
@rem   Old results are removed first, so that a failed build
@rem   never leaves a previous "Passed" behind.
@rem
@rem   TestPublicNLog is a separate project, because LogIF selects
@rem   the logging library only once per process (LogLib=nlog).
@rem   TestPublicDb is a separate project, because it needs databases
@rem   (SQL Server is required; the others are skipped when not reachable).
@rem   Each project has its own results folder, so that the
@rem   coverage of one project is not taken for the other.
@rem --------------------------------------------------
set NUNIT_DIR=Frameworks\Tests\NUnit
@rem Keep the .sln path literal: SelectTargets.ps1 reads it from this file
@rem to find which files this batch builds (a %%VAR%% in it cannot be resolved).
set NUNIT_SLN=Frameworks\Tests\NUnit\TestPublic\TestPublic.sln
set NUNIT_RESULTS=%NUNIT_DIR%\TestResults

if exist "%NUNIT_RESULTS%" rmdir /s /q "%NUNIT_RESULTS%"

dotnet restore "%NUNIT_SLN%"
dotnet msbuild %COMMANDLINE% "%NUNIT_SLN%"

call :RunTests TestPublic
call :RunTests TestPublicNLog
call :RunTests TestPublicDb

pause

rem -------------------------------------------------------
endlocal
goto :EOF

@rem --------------------------------------------------
@rem Run one test project for net48, then for net10.0 with coverage.
@rem   %1 : project name (folder name = csproj name)
@rem --------------------------------------------------
:RunTests
@echo --------------------------------------------------
@echo Test the %1(48).
@echo --------------------------------------------------
dotnet test "%NUNIT_DIR%\%1\%1.csproj" --no-build -c %BUILD_CONFIG% -f net48 --results-directory "%NUNIT_RESULTS%\%1" --logger "trx;LogFileName=%1_net48.trx"

@echo --------------------------------------------------
@echo Test the %1(100) with code coverage.
@echo --------------------------------------------------
dotnet test "%NUNIT_DIR%\%1\%1.csproj" --no-build -c %BUILD_CONFIG% -f net10.0 --results-directory "%NUNIT_RESULTS%\%1" --logger "trx;LogFileName=%1_net10.0.trx" --settings "%NUNIT_DIR%\TestPublic\coverlet.runsettings" --collect "XPlat Code Coverage"
goto :EOF
