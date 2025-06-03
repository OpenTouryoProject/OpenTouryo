setlocal

@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off
@chcp 65001

@rem --------------------------------------------------
@rem Get the path to the executable file.
@rem --------------------------------------------------
set CURRENT_DIR="%~dp0"

@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------
call %CURRENT_DIR%z_Common.bat

rem --------------------------------------------------
rem Batch build of ServiceInterface(ASPNETWebServiceCore).
rem --------------------------------------------------
rem dotnet restore "Frameworks\Infrastructure\ServiceInterface\ASPNETWebServiceCore\ASPNETWebServiceCore.sln"
rem dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\ServiceInterface\ASPNETWebServiceCore\ASPNETWebServiceCore.sln"

echo Core系のBinarySerializeの完全廃止対応

pause

rem -------------------------------------------------------
endlocal
