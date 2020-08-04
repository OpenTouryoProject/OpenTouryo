set CURRENTDIR=%cd%

cd "CS"
echo | call 0_ExecAllBat.bat > ..\Build_CS.log

cd %CURRENTDIR%
move /Y *.log .\BackupCICDLog

cd "CS"
echo | call y_Build_TestCode.bat > ..\Test_CS.log

cd %CURRENTDIR%
move /Y *.log .\BackupCICDLog

cd "VB"
echo | call 0_ExecAllBat.bat > ..\Build_VB.log

cd %CURRENTDIR%
move /Y *.log .\BackupCICDLog