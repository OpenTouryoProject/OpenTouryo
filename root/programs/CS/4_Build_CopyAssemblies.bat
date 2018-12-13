@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Copy for build local repository.
@rem --------------------------------------------------
xcopy /E /Y "Frameworks\Infrastructure\Build_net46" "Frameworks\Infrastructure\Build\"

@rem --------------------------------------------------
@rem Copy for build other repositories.
@rem --------------------------------------------------
xcopy /E /Y "Frameworks\Infrastructure\Build_net45" "C:\OpenTouryoAssemblies\Build_net45\"
xcopy /E /Y "Frameworks\Infrastructure\Build_net46" "C:\OpenTouryoAssemblies\Build_net46\"
xcopy /E /Y "Frameworks\Infrastructure\Build_net47" "C:\OpenTouryoAssemblies\Build_net47\"
xcopy /E /Y "Frameworks\Infrastructure\Build_netstandard20" "C:\OpenTouryoAssemblies\Build_netstandard20\"
xcopy /E /Y "Frameworks\Infrastructure\Build_netcore20" "C:\OpenTouryoAssemblies\Build_netcore20\"