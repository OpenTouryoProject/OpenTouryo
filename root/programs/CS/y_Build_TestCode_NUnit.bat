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
@rem   One csproj targets both net48 and net10.0.
@rem   The results (*.trx) are judged by 2_RunAllTests.ps1.
@rem   Old results are removed first, so that a failed build
@rem   never leaves a previous "Passed" behind.
@rem --------------------------------------------------
set NUNIT_SLN=Frameworks\Tests\NUnit\TestPublic\TestPublic.sln
set NUNIT_PROJ=Frameworks\Tests\NUnit\TestPublic\TestPublic.csproj
set NUNIT_RESULTS=Frameworks\Tests\NUnit\TestResults

if exist "%NUNIT_RESULTS%" rmdir /s /q "%NUNIT_RESULTS%"

dotnet restore "%NUNIT_SLN%"
dotnet msbuild %COMMANDLINE% "%NUNIT_SLN%"

@echo --------------------------------------------------
@echo Test the TestPublic(48).
@echo --------------------------------------------------
dotnet test "%NUNIT_PROJ%" --no-build -c %BUILD_CONFIG% -f net48 --results-directory "%NUNIT_RESULTS%" --logger "trx;LogFileName=TestPublic_net48.trx"

@echo --------------------------------------------------
@echo Test the TestPublic(100) with code coverage.
@echo --------------------------------------------------
dotnet test "%NUNIT_PROJ%" --no-build -c %BUILD_CONFIG% -f net10.0 --results-directory "%NUNIT_RESULTS%" --logger "trx;LogFileName=TestPublic_net10.0.trx" --settings "Frameworks\Tests\NUnit\TestPublic\coverlet.runsettings" --collect "XPlat Code Coverage"

pause

rem -------------------------------------------------------
endlocal
