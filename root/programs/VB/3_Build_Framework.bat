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
rem Output xcopy after you build the batch Infrastructure(AllComponent)
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\AllComponent.sln"

xcopy /E /Y "Frameworks\Infrastructure\Business\bin\%BUILD_CONFIG%" "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\"
xcopy /E /Y "Frameworks\Infrastructure\CustomControl\bin\%BUILD_CONFIG%" "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\"

xcopy /E /Y "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%" "Frameworks\Infrastructure\Build\"

pause

rem --------------------------------------------------
rem Output xcopy after you build the batch Infrastructure(RichClientComponent)
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\RichClientComponent.sln"

xcopy /E /Y "Frameworks\Infrastructure\Framework\RichClient\bin\%BUILD_CONFIG%" "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\"
xcopy /E /Y "Frameworks\Infrastructure\Business\RichClient\bin\%BUILD_CONFIG%" "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\"
xcopy /E /Y "Frameworks\Infrastructure\CustomControl\RichClient\bin\%BUILD_CONFIG%" "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\"

xcopy /E /Y "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%" "Frameworks\Infrastructure\Build\"

pause

rem --------------------------------------------------
rem Delete the System.Web.MVC.dll after the bulk copy
rem --------------------------------------------------
del "Frameworks\Infrastructure\Build\System.Web.MVC.*"
del "Frameworks\Infrastructure\Temp\%BUILD_CONFIG%\System.Web.MVC.*"

rem -------------------------------------------------------
endlocal
