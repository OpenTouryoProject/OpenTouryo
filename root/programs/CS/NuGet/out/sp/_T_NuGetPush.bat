setlocal
@echo off

@rem --------------------------------------------------
@rem Pass the API key through the NUGET_API_KEY environment variable (#531).
@rem
@rem "nuget.exe SetApiKey" used to be written here, with the key pasted in
@rem before the run and reverted afterwards. That was dangerous twice over.
@rem
@rem   - This file is tracked by Git. A key left in it lands in a commit.
@rem   - SetApiKey persists the key into %AppData%\NuGet\NuGet.Config.
@rem     Reverting this file does not remove it from there.
@rem
@rem An environment variable disappears when the console is closed,
@rem so there is nothing to clean up. Run this first, in the same console:
@rem
@rem   set NUGET_API_KEY=<the API key issued at nuget.org>
@rem
@rem The symbol server needs -SymbolApiKey. -ApiKey alone is NOT used for it,
@rem and the push of the .snupkg fails with 403 (#531).
@rem
@rem Scope the key to the target packages, give it the shortest expiry,
@rem and delete it at nuget.org once it has been used.
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem       Non-ASCII comments are decoded with the console code page,
@rem       which misaligns the parser and executes comment fragments.
@rem --------------------------------------------------
if not defined NUGET_API_KEY (
  echo [ERROR] The environment variable NUGET_API_KEY is not set.
  echo         set NUGET_API_KEY=^<the API key issued at nuget.org^>
  pause
  exit /b 1
)
@rem A .snupkg next to the .nupkg is published together with it.
@rem -source is required unless NuGet.Config defines DefaultPushSource.
@rem
@rem This is a WILDCARD. An older version left in out\sp is published too.
@rem Check the contents of the folder before running.
"..\..\..\..\nuget.exe" push Erutcurtsarfni.Oyruot.Public.*.nupkg -ApiKey %NUGET_API_KEY% -SymbolApiKey %NUGET_API_KEY% -source https://api.nuget.org/v3/index.json

@rem The symbol package therefore needs no explicit push.
@rem If the .nupkg was already published, its push fails with 409 and the
@rem symbols are never reached. Push the .snupkg on its own in that case:
@rem   "..\..\..\..\nuget.exe" push Erutcurtsarfni.Oyruot.Public.*.snupkg -ApiKey %NUGET_API_KEY% -SymbolApiKey %NUGET_API_KEY% -source https://api.nuget.org/v3/index.json

pause