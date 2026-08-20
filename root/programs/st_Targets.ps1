<#
.SYNOPSIS
    疎通テストの対象定義

.DESCRIPTION
    CS 版・VB 版の対象一覧。

    **他の st_*.ps1 が定義した関数と変数を参照する。**
    （$batchArgs / $mvcLoginFlow / Start-ApiWeb など）
    このため、ドット ソースの順序は最後でなければならない。

    **3_SmokeTest.ps1 からドット ソースで読まれる。**
    単体では動かない（#571 で分割）。
#>

$targetsCS = @(
    # --- DaoGen_Tool（墨壺）の CUI モード ---
    # #508 で追加された /HELP と /CUI。GUI 側の確認は手作業に残る。
    # DAODEFGEN → DAOSQLGEN の順に実行し、前段の出力を後段の入力に使う。
    @{
        Name = "DaoGen_Tool /HELP (net48)";        Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\OpenTouryo.DaoGen_Tool.exe"
        Args = @("/HELP");  Expect = 'DaoGen_Tool（D層自動生成ツール／墨壺）'
    }
    @{
        Name = "DaoGen_Tool DAODEFGEN (net48)";    Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoDefArgs48;  Expect = '生成が完了しました。'
        Pre = $prepareDaoGen48;  Verify = $verifyDaoDef48
    }
    @{
        Name = "DaoGen_Tool DAOSQLGEN (net48)";    Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoSqlArgs48;  Expect = '生成が完了しました。'
        Verify = $verifyDaoGen48
    }
    @{
        Name = "DaoGen_Tool /HELP (net10.0)";      Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
        Args = @("/HELP");  Expect = 'DaoGen_Tool（D層自動生成ツール／墨壺）'
    }
    @{
        Name = "DaoGen_Tool DAODEFGEN (net10.0)";  Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoDefArgsCore;  Expect = '生成が完了しました。'
        Pre = $prepareDaoGenCore;  Verify = $verifyDaoDefCore
    }
    @{
        Name = "DaoGen_Tool DAOSQLGEN (net10.0)";  Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DaoGen_Tool\bin\Debug\net10.0-windows7.0\OpenTouryo.DaoGen_Tool.exe"
        Args = $daoSqlArgsCore;  Expect = '生成が完了しました。'
        Verify = $verifyDaoGenCore
    }

    # --- DeployZipPackWithHTTP の CUI モード ---
    # #528 で /MFTGEN を追加し、生成 → 配布 → 配置 まで CUI で通せるようになった。
    # マニュフェストを作り、IIS Express で配信し、実際に配置して突き合わせる。
    # ※ /NB を付けないと、配置した EXE が起動して止まる。
    @{
        Name = "DeployZip /MFTGEN (net48)";        Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $mftGenArgs48;  Expect = 'マニュフェスト ファイルを生成しました。'
        Pre = $prepareDeploy48;  Verify = $verifyMftGen48
    }
    @{
        Name = "DeployZip 配置 (net48)";           Bat = "4_Build_Framework_Tool.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $deployArgs;  Expect = '履歴に新規追加しました。'
        Pre = $startDeployWeb48;  Verify = $verifyDeploy48
    }
    @{
        Name = "DeployZip /MFTGEN (net10.0)";      Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\net10.0-windows7.0\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $mftGenArgsCore;  Expect = 'マニュフェスト ファイルを生成しました。'
        Pre = $prepareDeployCore;  Verify = $verifyMftGenCore
    }
    @{
        Name = "DeployZip 配置 (net10.0)";         Bat = "4_Build_Framework_ToolCore.bat"
        Exe  = "Frameworks\Tools\DeployZipPackWithHTTP\bin\Debug\net10.0-windows7.0\OpenTouryo.DeployZipPackWithHTTP.exe"
        Args = $deployArgs;  Expect = '履歴に新規追加しました。'
        Pre = $startDeployWebCore;  Verify = $verifyDeployCore
    }

    # --- 通信制御の接続オプション（#546） ---
    #
    # CallController の接続オプション（ProxyUrl / UserName / UserAgent / Compression 等）が、
    # 実際の HTTP 要求に反映されているかを見る。
    #
    # ＜外部環境が要らない＞
    #   オリジンとプロキシを、テスト側が TcpListener で自前に立てる。
    #   1 プロセスに閉じているので、起動・停止の面倒を見る必要がない。
    #
    # ＜net48 だけ＞
    #   ASP.NET WebAPI の経路が .NET Framework 限定である（BinarySerialize を使うため）。
    #
    # ＜判定＞
    #   対象側が項目ごとに OK / NG を出し、末尾に件数を出す。
    @{
        Name = "TestTransmission (net48)";  Bat = "y_Build_TestTransmission.bat"
        Exe  = "Frameworks\Tests\TestTransmission\net48\bin\Debug\TestTransmissionFx.exe"
        Expect = 'NG : 0 件'
    }

    # --- バッチ (net48) ---
    @{
        Name = "SimpleBatch_sample (net48)";      Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\SimpleBatch_sample\bin\Debug\SimpleBatch_sample.exe"
        Args = $batchArgs;  Expect = '\d+件のデータがあります'
    }
    @{
        Name = "RerunnableBatch_sample (net48)";  Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample\bin\Debug\RerunnableBatch_sample.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample2 (net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample2\bin\Debug\RerunnableBatch_sample2.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample3 (net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample3\bin\Debug\RerunnableBatch_sample3.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }

    # --- バッチ (net10.0) ---
    @{
        Name = "SimpleBatch_sample (net10.0)";      Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\SimpleBatch_sample\bin\Debug\net10.0\SimpleBatch_sample.dll"
        Args = $batchArgs;  Expect = '\d+件のデータがあります'
    }
    @{
        Name = "RerunnableBatch_sample (net10.0)";  Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\RerunnableBatch_sample\bin\Debug\net10.0\RerunnableBatch_sample.dll"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample2 (net10.0)"; Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\RerunnableBatch_sample2\bin\Debug\net10.0\RerunnableBatch_sample2.dll"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample3 (net10.0)"; Bat = "5_Build_BatCore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\Bat_sample\RerunnableBatch_sample3\bin\Debug\net10.0\RerunnableBatch_sample3.dll"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }

    # --- CLI (net10.0) ---
    # net48 版は System.CommandLine / Sharprompt の .NET Fx サポート終了により
    # ドロップされている（5_Build_CLI_sample.bat 参照）。
    # interactive サブコマンドは Prompt を使うため対象外とし、非対話のものを使う。
    @{
        Name = "Simple_CLI (net10.0)";              Bat = "5_Build_CLICore_sample.bat"
        Exe  = "Samples4NetCore\Legacy\CLI_sample\Simple_CLI\Simple_CLI\bin\Debug\net10.0\Simple_CLI.dll"
        Args = @("cmd1", "--an-int", "123");  Expect = 'Sub command cmd1: 123'
    }

    # --- DTO を使用したバッチ更新（WebAPI Client）（#570） ---
    #
    # **DataTable を DTTables 経由で JSON にして往復させ、
    #   RowState と Original が保たれることを、HTTP 越しに確かめる。**
    #
    # ＜対象自身は EXE＞
    #   相手の WebAPI は Pre で起動し、Verify の最後で止める。
    #
    # ＜net48 と net10.0 で同じクライアントを使う＞
    #   接続先を引数で切り替えるだけ。応答の形も揃えてある。
    #
    # ＜判定＞
    #   対象側が項目ごとに OK / NG を出し、末尾に件数を出す。
    @{
        Name = "TestWebAPIClient (net48)";   Bat = "y_Build_TestWebAPIClient.bat"
        Exe  = "Frameworks\Tests\TestWebAPIClient\net48\bin\Debug\TestWebAPIClientFx.exe"
        Args = @("http://localhost:51087")
        Expect = 'NG : 0 件'
        Pre = { if (-not (Start-ApiWeb "net48" 51087)) { throw "WebAPI のホストを起動できません（port 51087）" } }
        Verify = { Stop-ApiWeb; return $true }
    }
    @{
        Name = "TestWebAPIClient (net10.0)"; Bat = "y_Build_TestWebAPIClient.bat"
        Exe  = "Frameworks\Tests\TestWebAPIClient\net48\bin\Debug\TestWebAPIClientFx.exe"
        Args = @("http://localhost:51088")
        Expect = 'NG : 0 件'
        Pre = { if (-not (Start-ApiWeb "net10.0" 51088)) { throw "WebAPI のホストを起動できません（port 51088）" } }
        Verify = { Stop-ApiWeb; return $true }
    }

    # --- Web アプリ ---
    @{
        Name = "WebForms_Sample (net48)"; Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51082
        Site = "Samples\WebApp_sample\WebForms_Sample\WebForms_Sample"
        Need = "aspnet_state"
        Flow = $webFormsFlow
    }
    @{
        Name = "MVC_Sample (net48)";      Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51081
        Site = "Samples\WebApp_sample\MVC_Sample\MVC_Sample"
        Need = "aspnet_state"
        Flow = $mvcLoginFlow
    }
    @{
        Name = "MVC_Sample (net10.0)";    Bat = "10_Build_WebAppCore_sample.bat"
        Kind = "Web";  WebHost = "Kestrel";  Port = 51083
        Exe  = "Samples4NetCore\Backend\MVC_Sample\MVC_Sample\bin\Debug\net10.0\MVC_Sample.dll"
        Flow = $mvcLoginFlow
    }
)

# ------------------------------------------------------------------
# VB 版の対象
# ------------------------------------------------------------------
# VB にあるのは Bat_sample と WebApp_sample だけで、Core 版・CLI_sample・
# ツール（Frameworks\Tools）は無い（#542）。
#
# 判定（Args / Pre / Verify / Flow）は C# 版と同じものを使い回す。
# 上の定義と同じ変数を指しているので、片方だけ直すことができない。
#
# ＜ポートを分ける＞
#   -Lang Both では C# 版と続けて起動するため、51081/51082 とは別にする。
$targetsVB = @(
    # --- バッチ (net48) ---
    @{
        Name = "SimpleBatch_sample (VB net48)";      Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\SimpleBatch_sample\bin\Debug\SimpleBatch_sample.exe"
        Args = $batchArgs;  Expect = '\d+件のデータがあります'
    }
    @{
        Name = "RerunnableBatch_sample (VB net48)";  Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample\bin\Debug\RerunnableBatch_sample.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample2 (VB net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample2\bin\Debug\RerunnableBatch_sample2.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }
    @{
        Name = "RerunnableBatch_sample3 (VB net48)"; Bat = "5_Build_Bat_sample.bat"
        Exe  = "Samples\Bat_sample\RerunnableBatch_sample3\bin\Debug\RerunnableBatch_sample3.exe"
        Args = $batchArgs;  Pre = $clearOrders2;  Verify = $verifyOrders2
    }

    # --- Web アプリ ---
    @{
        Name = "MVC_Sample (VB net48)";      Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51085
        Site = "Samples\WebApp_sample\MVC_Sample\MVC_Sample"
        Need = "aspnet_state"
        Flow = $mvcLoginFlow
    }
    @{
        Name = "WebForms_Sample (VB net48)"; Bat = "10_Build_WebApp_sample.bat"
        Kind = "Web";  WebHost = "IISExpress";  Port = 51086
        Site = "Samples\WebApp_sample\WebForms_Sample\WebForms_Sample"
        Need = "aspnet_state"
        Flow = $webFormsFlow
    }
)
