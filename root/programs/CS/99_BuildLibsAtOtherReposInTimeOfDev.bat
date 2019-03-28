@echo off

@rem 本バッチファイルの作成にあたり、以下のサイトを参考にしました。
@rem 【Bat】【vim】香り屋Vimをダウンロードしてインストールまでするbatファイル - Qiita
@rem https://qiita.com/koryuohproject/items/beed1a28ad6a1f60256d

setlocal

@rem ZIPファイル名
set zipfilename=Temp.zip

@rem GitHubのZIPパス
set srcUrl=https://github.com/OpenTouryoProject/OpenTouryo/archive/develop.zip

@rem 解凍ディレクトリ
set extDir=%CD%

@rem 一時ディレクトリ
set tmpDir=Temp

:Download
@rem ダウンロードされたZIPファイルがあるなら解凍
if exist %extDir%\%zipfilename% GOTO Extract

@rem ZIPファイルのダウンロード
@powershell -NoProfile -ExecutionPolicy Bypass -Command "$d=new-object System.Net.WebClient; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12; $d.Proxy.Credentials=[System.Net.CredentialCache]::DefaultNetWorkCredentials; $d.DownloadFile('%srcUrl%','%extDir%/%zipfilename%')"

:Extract
@rem 一時ディレクトリがあるならビルドへ
if exist %extDir%\%tmpDir% GOTO Build

@rem ZIPファイルを一時ディレクトリに解凍
@powershell -NoProfile -ExecutionPolicy Bypass -Command "expand-archive %zipfilename%"

:Build
@rem ビルドがあるならコピーへ
if exist "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore30" GOTO Xcopy

@rem batファイルを使用してビルド
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
@rem ビルド出力をコピー
cd %extDir%
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net45" "OpenTouryoAssemblies\Build_net45\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net46" "OpenTouryoAssemblies\Build_net46\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_net47" "OpenTouryoAssemblies\Build_net47\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore20" "OpenTouryoAssemblies\Build_netcore20\"
xcopy /Y /E "Temp\OpenTouryo-develop\root\programs\CS\Frameworks\Infrastructure\Build_netcore30" "OpenTouryoAssemblies\Build_netcore30\"

pause

:EOF
endlocal