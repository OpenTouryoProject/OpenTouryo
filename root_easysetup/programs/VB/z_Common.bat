@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------

@rem --------------------------------------------------
@rem Specifying Build tool.
@rem --------------------------------------------------
set BUILDFILEPATH="C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.com"

set BUILDFILEPATH2.0="C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\devenv.com"
set BUILDFILEPATH3.5="C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.0="C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.5="C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.5.1="C:\Program Files\Microsoft Visual Studio 12.0\Common7\IDE\IDE\devenv.com"

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo --------------------------------------------------
set BUILD_CONFIG=Debug

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
set COMMANDLINE=/build %BUILD_CONFIG%