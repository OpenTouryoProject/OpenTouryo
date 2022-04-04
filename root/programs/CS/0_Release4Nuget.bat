echo | call 1_DeleteDir.bat
echo | call 1_DeleteFile.bat

@echo on
timeout 5

echo | call 2_Build_NuGet_net45.bat
echo | call 2_Build_NuGet_net46.bat
echo | call 2_Build_NuGet_net47.bat
echo | call 2_Build_NuGet_net48.bat
echo | call 2_Build_NuGet_netstd20.bat
echo | call 2_Build_NuGet_netstd21.bat
echo | call 2_Build_NuGet_nettcore30.bat
echo | call 2_Build_NuGet_nettcore50.bat
echo | call 2_Build_NuGet_nettcore60.bat

echo | call 4_Build_CopyAssemblies.bat

rem --------------------------------------------------
rem Change the packages.config.
rem --------------------------------------------------
call %CURRENT_DIR%z_ChangePackages_net46.bat
