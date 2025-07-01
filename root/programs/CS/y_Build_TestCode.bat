@rem --------------------------------------------------
@rem TestCode -> TestCodeFx, TestCodeCore
@rem --------------------------------------------------
echo | call y_Build_TestCode_Public.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem EncAndDecUtil -> 暗号GUIでビルドだけ
@rem --------------------------------------------------
echo | call y_Build_TestCode_SecGUI.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem EncAndDecUtilCUI -> 暗号CUIでビルド&テスト
@rem --------------------------------------------------
echo | call y_Build_TestCode_SecCUI.bat

@echo on
timeout 5

@rem --------------------------------------------------
@rem TestBatch -> SimpleBatchでDB接続
@rem --------------------------------------------------
echo | call y_Build_TestCode_Batch.bat

@echo on
timeout 5
