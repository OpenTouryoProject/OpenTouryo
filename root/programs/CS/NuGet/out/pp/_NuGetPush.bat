setlocal
@echo off
chcp 65001 >nul

@rem --------------------------------------------------
@rem API キーは環境変数 NUGET_API_KEY で渡す（#531）。
@rem
@rem 以前は nuget.exe SetApiKey を使っていたが、次の 2 点で危険だった。
@rem
@rem   ・このファイルは Git で追跡されている。キーを直書きすると、
@rem     戻し忘れたままコミットに載る。
@rem   ・SetApiKey はキーを %AppData%\NuGet\NuGet.Config へ永続化する。
@rem     このファイルをプレースホルダに戻しても、そちらは消えない。
@rem
@rem 環境変数なら、コンソールを閉じれば消えるため後始末が要らない。
@rem 実行前に、同じコンソールで次を実行する。
@rem
@rem   set NUGET_API_KEY=＜nuget.org で発行したキー＞
@rem
@rem キーは**スコープと有効期限を絞り、使用後に nuget.org 側で削除**すること。
@rem --------------------------------------------------
if not defined NUGET_API_KEY (
  echo [ERROR] 環境変数 NUGET_API_KEY が設定されていません。
  echo         set NUGET_API_KEY=＜nuget.org で発行したキー＞
  pause
  exit /b 1
)
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Public.*.nupkg -ApiKey %NUGET_API_KEY% -source https://api.nuget.org/v3/index.json
"..\..\..\..\nuget.exe" push Touryo.Infrastructure.Framework.*.nupkg -ApiKey %NUGET_API_KEY% -source https://api.nuget.org/v3/index.json

pause