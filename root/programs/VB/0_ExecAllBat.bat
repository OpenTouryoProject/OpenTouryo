set CURRENTDIR=%cd%
cd "..\CS"

@echo on
timeout 5

echo | call 2_Build_NuGet_net48.bat

@echo on
timeout 5

cd %CURRENTDIR%

echo | call 1_DeleteDir.bat
echo | call 1_DeleteFile.bat

@echo on
timeout 5

echo | call 1_GetLibrariesFromCS.bat

@echo on
timeout 5

echo | call 3_Build_Business_net48.bat
echo | call 3_Build_BusinessRichClient_net48.bat
echo | call 4_Build_CopyAssemblies.bat

@echo on
timeout 5

echo | call 5_Build_Bat_sample.bat
echo | call 5_Build_2CS_sample.bat
echo | call 6_Build_WSSrv_sample.bat
echo | call 7_Build_Framework_WS.bat
echo | call 8_Build_WSClntWin_sample.bat
echo | call 9_Build_WSClntWPF_sample.bat
echo | call 10_Build_WebApp_sample.bat

@echo on
timeout 5