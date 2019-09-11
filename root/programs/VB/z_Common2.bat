@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------

@rem --------------------------------------------------
@rem Set Program Files path
@rem --------------------------------------------------
reg Query "HKLM\Hardware\Description\System\CentralProcessor\0" | find /i "x86" > NUL && set PROGRAM_FILES=C:\Program Files||set PROGRAM_FILES=C:\Program Files (x86)

@rem --------------------------------------------------
@rem Specifying Build tool.
@rem --------------------------------------------------
set BUILDFILEPATH2.0="%PROGRAM_FILES%\Microsoft Visual Studio 8\Common7\IDE\devenv.com"
set BUILDFILEPATH3.5="%PROGRAM_FILES%\Microsoft Visual Studio 9.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.0="%PROGRAM_FILES%\Microsoft Visual Studio 10.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.5="%PROGRAM_FILES%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.5.1="%PROGRAM_FILES%\Microsoft Visual Studio 12.0\Common7\IDE\IDE\devenv.com"
set BUILDFILEPATH4.6="%PROGRAM_FILES%\Microsoft Visual Studio 14.0\Common7\IDE\devenv.com"
set BUILDFILEPATH4.7="%PROGRAM_FILES%\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe"
set BUILDFILEPATH4.8="%PROGRAM_FILES%\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe"

set BUILDFILEPATH=BUILDFILEPATH4.8

@echo --------------------------------------------------
@echo The choice of build configuration (Debug / Release).
@echo --------------------------------------------------
set BUILD_CONFIG=Debug

@echo --------------------------------------------------
@echo Creating a build command.
@echo --------------------------------------------------
set COMMANDLINE=/build %BUILD_CONFIG%

@echo --------------------------------------------------
@echo Set the proxy settings of Nuget.
@echo --------------------------------------------------
@rem set http_proxy=http://[username]:[password]@[proxy fqdn or ip address]