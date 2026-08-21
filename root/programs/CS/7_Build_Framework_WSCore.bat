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
rem Batch build of ServiceInterface (.NET Core).
rem --------------------------------------------------
rem Nothing to build here.
rem   The .NET Core edition of ServiceInterface (ASPNETWebServiceCore) was dropped
rem   when BinarySerialize was abolished for Core.
rem   ServiceInterface now holds ASPNETWebService (net48) and WCFService only.
rem
rem Keep this batch : 1_BuildAll.ps1 runs it as the Framework_WSCore step.
rem   Removing it would make a skipped step look like a failed one.

echo Core系のBinarySerializeの完全廃止対応

pause

rem -------------------------------------------------------
endlocal
