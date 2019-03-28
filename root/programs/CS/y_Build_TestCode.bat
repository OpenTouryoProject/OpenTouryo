echo | call y_Build_TestCode_Public.bat

@echo on
timeout 5

echo | call y_Build_TestCode_SecGUI.bat

@echo on
timeout 5

echo | call y_Build_TestCode_SecCUI.bat