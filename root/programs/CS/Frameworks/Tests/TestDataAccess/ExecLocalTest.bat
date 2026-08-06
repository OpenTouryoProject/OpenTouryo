setlocal

@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Cross DB test (for local execution only).
@rem
@rem Start the containers of LocalServicesOnDocker first.
@rem This can not run on the CI because each DBMS runs on a Linux container.
@rem See root\programs\TESTING.md for the details.
@rem --------------------------------------------------

@rem --------------------------------------------------
@rem Get the path to the executable file.
@rem --------------------------------------------------
set CURRENT_DIR=%~dp0
cd /d "%CURRENT_DIR%"

@rem --------------------------------------------------
@rem About the output files.
@rem
@rem Write to ResultLocal*.txt, never to Result*.txt.
@rem Result*.txt are the expected values of 2_RunAllTests.ps1,
@rem which are recorded with /MODE SQLONLY.
@rem The output of /MODE LOCAL varies with the running DBMS,
@rem so overwriting them breaks the expected values.
@rem --------------------------------------------------

@rem --------------------------------------------------
@rem About "echo. |".
@rem
@rem Program.cs calls Console.ReadKey() at the end.
@rem Without redirecting the stdin, it really waits for a key press.
@rem --------------------------------------------------

@echo --------------------------------------------------
@echo net48 : SQL Server / Oracle / MySQL
@echo --------------------------------------------------
echo. | "net48\bin\Debug\TestDataAccessFx.exe" /MODE LOCAL > "%CURRENT_DIR%ResultLocal48.txt"

@echo --------------------------------------------------
@echo net10.0 : SQL Server / Oracle / MySQL / PostgreSQL
@echo --------------------------------------------------
cd /d "%CURRENT_DIR%core100\bin\Debug\net10.0"
echo. | dotnet "TestDataAccessCore.dll" -- /MODE LOCAL > "%CURRENT_DIR%ResultLocalCore100.txt"
cd /d "%CURRENT_DIR%"

@echo --------------------------------------------------
@echo Output : ResultLocal48.txt / ResultLocalCore100.txt
@echo --------------------------------------------------

pause

rem -------------------------------------------------------
endlocal