@rem --------------------------------------------------
@rem TestCode
@rem --------------------------------------------------
echo | call y_Build_TestCode_Public.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem EncAndDecUtil -> GUIでビルドだけ
@rem --------------------------------------------------
echo | call y_Build_TestCode_SecGUI.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem EncAndDecUtilCUI
@rem --------------------------------------------------
echo | call y_Build_TestCode_SecCUI.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem TestBatch
@rem --------------------------------------------------
echo | call y_Build_TestCode_Batch.bat

@echo on
timeout 5
