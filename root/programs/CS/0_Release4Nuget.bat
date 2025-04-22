echo | call 1_DeleteDir.bat
echo | call 1_DeleteFile.bat

@echo on
timeout 5

echo | call 2_Build_NuGet_net48.bat
echo | call 2_Build_NuGet_netstd20.bat
echo | call 2_Build_NuGet_nettcore80.bat

echo | call 4_Build_CopyAssemblies.bat
