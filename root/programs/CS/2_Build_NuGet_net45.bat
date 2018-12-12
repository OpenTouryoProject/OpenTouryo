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
rem Change the packages.config.
rem --------------------------------------------------
call %CURRENT_DIR%z_ChangePackages_net45.bat

rem --------------------------------------------------
rem Build the batch Infrastructure(Nuget45)
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Infrastructure\Nuget_net45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Nuget_net45.sln"

..\nuget.exe restore "Frameworks\Infrastructure\Nuget_RichClient_net45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Nuget_RichClient_net45.sln"

pause

rem --------------------------------------------------
rem Delete the System.Web.MVC.dll after the bulk copy
rem --------------------------------------------------
del "Frameworks\Infrastructure\Build\System.Web.MVC.*"
del "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\System.Web.MVC.*"

rem -------------------------------------------------------
endlocal
