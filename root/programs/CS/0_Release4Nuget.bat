@echo on
timeout 5

echo | call 1_DeleteDir.bat
echo | call 2_Build_NuGet_net48.bat

@echo on
timeout 5

echo | call 1_DeleteDir.bat
echo | call 2_Build_NuGet_nettcore80.bat

@echo on
timeout 5

echo | call 4_Build_CopyAssemblies.bat
