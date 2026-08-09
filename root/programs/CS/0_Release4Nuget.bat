@echo off
@rem --------------------------------------------------
@rem NuGet パッケージ用のビルド（#531）
@rem
@rem **DEBUG_TYPE=portable を渡す。** z_Common.bat は、呼び出し側が
@rem 設定済みならその値を尊重する。以前は z_Common.bat を手で書き換えて
@rem 戻す運用だったが、戻し忘れると full のまま公開してしまうため。
@rem
@rem   ・.snupkg は portable PDB でなければ受け付けられない
@rem   ・Source Link の情報も portable PDB に載る
@rem --------------------------------------------------
set DEBUG_TYPE=portable

@echo on
timeout 5

echo | call 1_DeleteDir.bat
echo | call 2_Build_NuGet_net48.bat

@echo on
timeout 5

echo | call 1_DeleteDir.bat
echo | call 2_Build_NuGet_netcore100.bat

@echo on
timeout 5

echo | call 4_Build_CopyAssemblies.bat
