setlocal
@echo off

@rem --------------------------------------------------
@rem Clear the NuGet working folders (#531).
@rem
@rem   _Cleanup.bat                        in\ and ALL packages in out\
@rem   _Cleanup.bat <package id prefix>    in\ and only that prefix
@rem
@rem /NOPAUSE skips the pause at the end, and may be given in either
@rem position (with or without a prefix).
@rem
@rem _NuGetPack.bat and _T_NuGetPack.bat call this before their xcopy, each
@rem passing its own prefix. Run it by hand for a full reset.
@rem
@rem Why this is done BEFORE packing rather than after:
@rem
@rem   in\   xcopy /E /Y only overwrites. It never deletes, so a file the
@rem         build no longer produces stays behind and can still be picked
@rem         up by a nuspec. in\net48 had accumulated satellite assemblies
@rem         from past dependencies in 14 languages.
@rem
@rem   out\  _NuGetPush.bat pushes by wildcard. An older version left behind
@rem         would be published together with the new one, and an
@rem         unpublished prerelease would go out unintentionally.
@rem
@rem The .txt files that explain each folder, and the _NuGetPush.bat files,
@rem are tracked by Git. The folders are therefore kept: only the generated
@rem file types and the subdirectories are removed, matching .gitignore.
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem --------------------------------------------------
set OT_PREFIX=%~1
set OT_NOPAUSE=

@rem /NOPAUSE may be the first argument, with no prefix given.
if /i "%OT_PREFIX%"=="/NOPAUSE" (
  set OT_PREFIX=
  set OT_NOPAUSE=1
)

if /i "%~2"=="/NOPAUSE" set OT_NOPAUSE=1

echo --------------------------------------------------

if defined OT_PREFIX (
  echo Clearing in\ and out\ packages named %OT_PREFIX%* .
) else (
  echo Clearing in\ and ALL packages in out\ .
)

echo --------------------------------------------------

@rem --------------------------------------------------
@rem in\ : the generated file types and the subdirectories.
@rem --------------------------------------------------
for %%d in (net48 net10.0 net10.0-windows) do if exist "%~dp0in\%%d" (
  del /q "%~dp0in\%%d\*.dll" "%~dp0in\%%d\*.pdb" "%~dp0in\%%d\*.xml" "%~dp0in\%%d\*.config" "%~dp0in\%%d\*.json" >nul 2>&1
  for /d %%s in ("%~dp0in\%%d\*") do rd /s /q "%%s"
)

@rem --------------------------------------------------
@rem out\ : the packages. sp is the one in use; pp is kept for the case
@rem where symbols are not registered.
@rem --------------------------------------------------
for %%o in (sp pp) do if exist "%~dp0out\%%o" (
  del /q "%~dp0out\%%o\%OT_PREFIX%*.nupkg" "%~dp0out\%%o\%OT_PREFIX%*.snupkg" >nul 2>&1
)

echo Done.

if defined OT_NOPAUSE goto :eof

pause