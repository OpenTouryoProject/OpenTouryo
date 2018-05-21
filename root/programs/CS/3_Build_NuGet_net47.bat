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
rem Make the Directory.
rem --------------------------------------------------
md "Frameworks\Infrastructure\Temp"
md "Frameworks\Infrastructure\Build"

rem --------------------------------------------------
rem Change the packages.config.
rem --------------------------------------------------
copy /Y "Frameworks\Infrastructure\Public\packages_net47.config" "Frameworks\Infrastructure\Public\packages.config"
copy /Y "Frameworks\Infrastructure\Public\Db\DamManagedOdp\packages_net47.config" "Frameworks\Infrastructure\Public\Db\DamManagedOdp\packages.config"
copy /Y "Frameworks\Infrastructure\Public\Db\DamPstGrS\packages_net47.config" "Frameworks\Infrastructure\Public\Db\DamPstGrS\packages.config"
copy /Y "Frameworks\Infrastructure\Public\Db\DamMySQL\packages_net47.config" "Frameworks\Infrastructure\Public\Db\DamMySQL\packages.config"

copy /Y "Frameworks\Infrastructure\Framework\packages_net47.config" "Frameworks\Infrastructure\Framework\packages.config"

rem --------------------------------------------------
rem Build the batch Infrastructure(Nuget47)
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Infrastructure\Nuget_net47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Nuget_net47.sln"

..\nuget.exe restore "Frameworks\Infrastructure\Nuget_RichClient_net47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Nuget_RichClient_net47.sln"

pause

rem --------------------------------------------------
rem Delete the System.Web.MVC.dll after the bulk copy
rem --------------------------------------------------
del "Frameworks\Infrastructure\Build\System.Web.MVC.*"
del "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\System.Web.MVC.*"

rem -------------------------------------------------------
endlocal
