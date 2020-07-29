set CURRENTDIR=%cd%

cd %CURRENTDIR%
cd "CS"
echo | call 0_ExecAllBat.bat > ..\Build_CS.log
echo | call y_Build_TestCode.bat > ..\Test_CS.log

cd %CURRENTDIR%
cd "VB"
echo | call 0_ExecAllBat.bat > ..\Build_VB.log
