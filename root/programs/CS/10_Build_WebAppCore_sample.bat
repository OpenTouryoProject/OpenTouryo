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
rem Batch build of MVC_Sample.
rem --------------------------------------------------

@rem npm / grunt によるクライアント ライブラリの復元は廃止した。
@rem （wwwroot\lib 配下をリポジトリに直接格納する方式に移行済み）

dotnet restore "Samples4NetCore\Backend\MVC_Sample\MVC_Sample.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Backend\MVC_Sample\MVC_Sample.sln"

pause

rem -------------------------------------------------------
endlocal
