@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Copy for build local repository.
@rem --------------------------------------------------
xcopy /E /Y "Frameworks\Infrastructure\Build_net46" "Frameworks\Infrastructure\Build\"
xcopy /E /Y "Frameworks\Infrastructure\Build_netcore20" "C:\OpenTouryoAssemblies\Build_netcore\"
