@echo off

@rem **************************************************
@rem [NOTE] This batch does not work now - it is not maintained.
@rem   The batches it calls for net45 / net46 / net47 / netstd20 /
@rem   netcore20 / netcore30 no longer exist.
@rem   The xcopy sources Build_net45 ... Build_netcore30 are not generated either.
@rem   The current targetFramework is net48 / netcore100.
@rem   A full review is required before using it.
@rem **************************************************

@rem This batch was written with reference to the following site.
@rem [Bat][vim] A batch file that downloads and installs Kaoriya Vim - Qiita
@rem https://qiita.com/koryuohproject/items/beed1a28ad6a1f60256d

setlocal

@rem ZIP file name
set zipfilename=Temp.zip

@rem ZIP path on GitHub
set srcUrl=https://github.com/OpenTouryoProject/OpenTouryo/archive/master.zip

@rem Extraction directory
set extDir=%CD%

@rem Temporary directory
set tmpDir=Temp

:Download
@rem If the downloaded ZIP file exists, extract it
if exist %extDir%\%zipfilename% GOTO Extract

@rem Download the ZIP file
@powershell -NoProfile -ExecutionPolicy Bypass -Command "$d=new-object System.Net.WebClient; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12; $d.Proxy.Credentials=[System.Net.CredentialCache]::DefaultNetWorkCredentials; $d.DownloadFile('%srcUrl%','%extDir%/%zipfilename%')"

:Extract
@rem If the temporary directory exists, go to the build
if exist %extDir%\%tmpDir% GOTO Build

@rem Extract the ZIP file into the temporary directory
@powershell -NoProfile -ExecutionPolicy Bypass -Command "expand-archive %zipfilename%"

:Build
@rem If the build exists, go to the copy
if exist "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore30" GOTO Xcopy

@rem Build with the batch files
cd "Temp\OpenTouryo-develop\root\programs\CS\"
echo | call 2_Build_NuGet_net45.bat
echo | call 2_Build_NuGet_net46.bat
echo | call 2_Build_NuGet_net47.bat
echo | call 2_Build_NuGet_netstd20.bat
echo | call 3_Build_Business_net45.bat
echo | call 3_Build_Business_net46.bat
echo | call 3_Build_Business_net47.bat
echo | call 3_Build_Business_netcore20.bat
echo | call 3_Build_Business_netcore30.bat

:Xcopy
@rem Copy the build output
cd %extDir%
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net45" "OpenTouryoAssemblies\Build_net45\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net46" "OpenTouryoAssemblies\Build_net46\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net47" "OpenTouryoAssemblies\Build_net47\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore20" "OpenTouryoAssemblies\Build_netcore20\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore30" "OpenTouryoAssemblies\Build_netcore30\"

pause

:EOF
endlocal