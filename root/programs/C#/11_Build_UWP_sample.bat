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
call %CURRENT_DIR%z_Common2.bat

rem --------------------------------------------------
rem Batch build of UWP_sample_Xaml.
rem --------------------------------------------------
..\nuget.exe restore "Samples\UWP_sample\UWP_sample_Xaml.sln"
%BUILDFILEPATH% %COMMANDLINE% "Samples\UWP_sample\UWP_sample_Xaml.sln"

pause

rem --------------------------------------------------
rem Batch build of UWP_sample_Html.
rem --------------------------------------------------
..\nuget.exe restore "Samples\UWP_sample\UWP_sample_Html.sln"
%BUILDFILEPATH% %COMMANDLINE% "Samples\UWP_sample\UWP_sample_Html.sln"

pause

rem -------------------------------------------------------
endlocal
