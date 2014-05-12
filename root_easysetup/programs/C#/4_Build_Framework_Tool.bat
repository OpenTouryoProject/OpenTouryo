setlocal

@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Save the value of the PATH environment variable.
@rem --------------------------------------------------
set ORG_PATH=%PATH% 

@rem --------------------------------------------------
@rem Get the path to the executable file.
@rem --------------------------------------------------
set CURRENT_DIR=%~dp0

@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------
call %CURRENT_DIR%z_Common.bat

rem --------------------------------------------------
rem Batch build of DPQuery_Tool.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\DPQuery_Tool\DPQuery_Tool.sln"

pause

rem --------------------------------------------------
rem Batch build of DaoGen_Tool.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\DaoGen_Tool\DaoGen_Tool.sln"

pause

rem --------------------------------------------------
rem Batch build of Deploy ZipPack With HTTP.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\DeployZipPackWithHTTP\DeployZipPackWithHTTP.sln"

pause

rem --------------------------------------------------
rem Batch build of TestEncAndDecProvider.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\Encryption\TestEncAndDecProvider\TestEncAndDecProvider.sln"

pause

rem --------------------------------------------------
rem Batch build of EncAndDecUtil.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\Encryption\EncAndDecUtil\EncAndDecUtil.sln"

pause

rem -------------------------------------------------------
endlocal
