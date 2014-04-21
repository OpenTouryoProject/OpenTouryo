@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------

@rem --------------------------------------------------
@rem Specifying Build tool.
@rem --------------------------------------------------
set BUILDFILEPATH="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

set BUILDFILEPATH2.0="C:\Windows\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe"
set BUILDFILEPATH3.5="C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"
set BUILDFILEPATH4.0="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo --------------------------------------------------
set BUILD_CONFIG=Debug

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
set COMMANDLINE=/p:Configuration=%BUILD_CONFIG%