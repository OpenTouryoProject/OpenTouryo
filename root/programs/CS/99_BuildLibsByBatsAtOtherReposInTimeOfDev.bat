@echo off

@rem 【Bat】【vim】香り屋Vimをダウンロードしてインストールまでするbatファイル - Qiita
@rem https://qiita.com/koryuohproject/items/beed1a28ad6a1f60256d

setlocal

@rem ZIPファイル名
set zipfilename=Temp.zip

@rem GitHubパス
set srcUrl=https://github.com/OpenTouryoProject/OpenTouryo/archive/develop.zip

@rem 解凍ディレクトリ
set extDir=%CD%

@rem 一時ディレクトリ
set tmpDir=Temp

:Download
@rem ダウンロードされたデータがあるなら展開へ
if exist %extDir%\%zipfilename% GOTO Extract

@powershell -NoProfile -ExecutionPolicy Bypass -Command "$d=new-object System.Net.WebClient; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12; $d.Proxy.Credentials=[System.Net.CredentialCache]::DefaultNetWorkCredentials; $d.DownloadFile('%srcUrl%','%extDir%/%zipfilename%')"

:Extract
@rem 一時ディレクトリがあるならビルドへ
if exist %extDir%\%tmpDir% GOTO Build

@powershell -NoProfile -ExecutionPolicy Bypass -Command "expand-archive %zipfilename%"

:Build
@rem ビルドがあるならコピーへ
if exist "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore20" GOTO Xcopy

cd "Temp\OpenTouryo-develop\root\programs\CS\"

call 2_Build_NuGet_net45.bat
call 2_Build_NuGet_net46.bat
call 2_Build_NuGet_net47.bat
call 2_Build_NuGet_netstd20.bat
call 3_Build_Business_net45.bat
call 3_Build_Business_net46.bat
call 3_Build_Business_net47.bat
call 3_Build_Business_netcore20.bat

:Xcopy
cd extDir
xcopy /E /Y "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net45" "OpenTouryoAssemblies\Build_net45\"
xcopy /E /Y "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net46" "OpenTouryoAssemblies\Build_net46\"
xcopy /E /Y "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net47" "OpenTouryoAssemblies\Build_net47\"
xcopy /E /Y "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore20" "OpenTouryoAssemblies\Build_netcore20\"

:EOF
endlocal