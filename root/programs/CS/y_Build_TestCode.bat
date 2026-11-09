@rem --------------------------------------------------
@rem TestCode -> TestCodeFx, TestCodeCore
@rem --------------------------------------------------
echo | call y_Build_TestCode_Public.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem EncAndDecUtil -> build only, with the crypto GUI
@rem --------------------------------------------------
echo | call y_Build_TestCode_SecGUI.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem EncAndDecUtilCUI -> build and test, with the crypto CUI
@rem --------------------------------------------------
echo | call y_Build_TestCode_SecCUI.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem TestBatch -> DB connection with SimpleBatch
@rem --------------------------------------------------
echo | call y_Build_TestCode_Batch.bat

@echo on
timeout 5
