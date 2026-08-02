# Open 棟梁
 
## 概要 
Open 棟梁は、.NET Framework と .NET Core をベースとしたアプリケーション フレームワークです。

このファイルの英語版は[こちら](README.md)から。

## 本体開発用プログラムの実行手順
- Open 棟梁の本体と同梱されるサンプルアプリの実行手順は以下のとおりです。
- Open 棟梁を使用したアプリ開発を行う場合のセットアップは[コチラ](https://github.com/OpenTouryoProject/OpenTouryoCodingAgentAssets/)をご参照下さい。
- コーディング エージェントで本体開発を行う場合は、先に [AGENTS.md](../AGENTS.md) をお読みください。

### 前提ツールのインストール
- あらかじめ、Visual Studio （若しくは Visual Studio Code と .NET系の SDK と Extension）をインストールしておいてください。  

- また、Open 棟梁がサポートしている DBMS 中から使用するものを準備してください。[LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker) が便利です。

- サポートしている データプロバイダは、[Touryo.Infrastructure.Public.Db](https://github.com/OpenTouryoProject/OpenTouryo/tree/develop/root/programs/CS/Frameworks/Infrastructure/Public/Db) から確認できます。

- 正式サポートされているデータプロバイダは「SQL/OLE/ODBC/ODP/MCN/NPG」です。

- 現在は除外されていますが「DB2/HiRDB/OracleClient」は雛形があるのでエージェントなどを活用してセルフ・サポートすることは可能です。

- 略号とADO.NETデータプロバイダの名前空間
  - SQL：Microsoft.Data.SqlClient
  - OLE：System.Data.Odbc
  - ODBC：System.Data.OleDb
  - ODP：Oracle.ManagedDataAccess.Client
  - MCN：MySql.Data.MySqlClient
  - NPG：Npgsql
  - DB2：IBM.Data.DB2
  - HiRDB：Hitachi.HiRDB
  - OracleClient：System.Data.OracleClient

### Open 棟梁の配置
「root」フォルダを、C ドライブ直下にコピーしてください。C ドライブ直下以外にコピーすると、Windows のファイルパスの最大文字長の制限により、ビルドに失敗することがあります。

### サンプルデータベースのセットアップ

#### [LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker)を活用

#### SQL Server  
サンプルアプリケーションの実行には、Northwind データベースが必要です。
以下のマイクロソフトのサイトから、Northwind データベースのセットアップ スクリプトをダウンロードし、インストールしてください。  

- Download: NorthWind and pubs Sample Databases for SQL Server 2000 - Microsoft Download Center  
  http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=23654

インストールが成功すると、C ドライブ直下に "SQL Server 2000 Sample Databases" フォルダが作成されます。  
SQL Server 2012 以降をお使いの場合は、このフォルダにある instnwnd.sql ファイルをエディタなどで開き、以下のコードをコメントアウトしてください。(SQL Server 2012 以降では sp_dboption システム ストアド プロシージャがないため)

```sql
exec sp_dboption 'Northwind','trunc. log on chkpt.','true'
exec sp_dboption 'Northwind','select into/bulkcopy','true'
```

コマンドプロンプトで、下記コマンドを実行してください。  
(以下のコマンドの中で、「SQLCMD.EXE」のフォルダパスは SQL Server のバージョンによって異なります。お使いのバージョンでのフォルダパスをご確認の上、コマンドを実行してください)
```bat
"C:\Program Files\Microsoft SQL Server\100\Tools\Binn\SQLCMD.EXE" -S localhost\SQLExpress -E -i "C:\SQL Server 2000 Sample Databases\instnwnd.sql"
```

#### SQL Server 以外
- 各 DBMS に、空のデータベースを作成してください。
- C:\root\files\resource\Sql\\[DBMS 名]\TestTable.txt を実行し、テスト用のテーブルを作成してください。

### プログラムのビルド
- Open 棟梁のプログラムをビルドするときは、ビルドバッチファイルを実行してビルドします。  

- ビルドバッチファイルは、以下のフォルダにあります。
  - C:\root\programs\
  - C:\root\programs\CS  
  - C:\root\programs\VB

#### ビルドバッチの構成

ビルドバッチは**ファイル名の先頭の番号がビルド順**を表しています。
番号の小さいものから順に、基盤 → ツール → サンプルの順で積み上がる構成です。

| 番号 | 役割 |
|---|---|
| `0_` | 一括実行（`0_ExecAllBat.bat` が以下を順に呼び出します） |
| `1_` | クリーン（`bin` / `obj` / `packages` などの削除） |
| `2_` | フレームワーク本体（NuGet パッケージ化の対象となるアセンブリ） |
| `3_` | Business 層（業務コードの親クラス／テンプレート） |
| `4_` | 参照用アセンブリのコピー、付属ツール |
| `5_` `6_` `8_` `10_` | 各種サンプルアプリケーション |
| `7_` | Web サービスの受け口（フレームワーク側） |
| `9_` | （C# 側では未使用。VB 側で WPF クライアントに使用） |
| `y_` | 単体テストのコード |
| `z_` | 共通処理（各バッチの先頭から呼ばれます。単体では実行しません） |

- **`0_ExecAllBat.bat` を実行すれば、基盤からサンプルまでが一括でビルドされます。**
  個別のバッチは、一部だけを作り直したいときに使います。
- **`y_`（単体テスト）は `0_ExecAllBat.bat` に含まれません。** 単体テストを動かす場合は、
  後述の `2_RunAllTests.ps1` を使うか、`y_` のバッチを個別に実行します。
- ファイル名に `Core` が付くもの、または `netcore100` を含むものが .NET 10.0 向け、
  付かないもの（`net48`）が .NET Framework 4.8 向けです。
- `1_` のクリーンは**繰り返し実行されます**。基盤とサンプルを別々のタイミングで
  作り直すためで、このため全体をビルドし終えた時点では、
  最後にビルドされたもの以外の中間生成物は残りません。

#### 共通処理（z_Common.bat）

- 各バッチは先頭で `z_Common.bat` を呼び、次を用意します。
  - **ビルドツールの解決** … `vswhere` で MSBuild を探します（エディションに依存しません）
  - **ビルド構成** … `BUILD_CONFIG`（Debug / Release）と `DEBUG_TYPE`
  - **NuGet の設定** … プロキシ、および復元時に使う MSBuild の明示

- `z_Common2.bat` は同じ役割の **devenv 版**です。MSBuild では通らないが  
  devenv なら通る、というケースに備えて残されているもので、通常は使いません。

- Open 棟梁 が利用するライブラリは、NuGet 経由でダウンロードします。このため、プロキシ環境では、正常に NuGet ライブラリがダウンロードできないことがあります。プロキシ環境をお使いの場合は、以下のように http_proxy 環境変数を定義してください。
    - C:\root\programs\CS\z_Common.bat および C:\root\programs\VB\z_Common.bat を、テキストエディタで開きます。
    - 既定では、http_proxy 環境変数の定義部分はコメントアウトされていますので、"@rem" を削除して、このコメントを解除します。
    - http_proxy 環境変数に、お使いのプロキシ情報を設定してください。

#### ビルド後の検証

ビルドが通ることの確認、単体テスト、サンプルの疎通確認は、
`C:\root\programs\` にあるスクリプトで行えます。いずれも**終了コードで合否が分かります**。

```powershell
cd C:\root\programs
.\0_RunAll.ps1          # 下記 3 本をまとめて実行
```

| スクリプト | 内容 |
|---|---|
| `1_BuildAll.ps1` | 全ビルド（`0_ExecAllBat.bat` 相当）。エラー・警告を集約して判定 |
| `2_RunAllTests.ps1` | 単体テストを実行し、結果を前回のものと比較 |
| `3_SmokeTest.ps1` | サンプルアプリケーションを起動して疎通を確認 |

手順と判定基準は、同じフォルダの
[`BUILDING.md`](programs/BUILDING.md) / [`TESTING.md`](programs/TESTING.md) /
[`SMOKETEST.md`](programs/SMOKETEST.md) を参照してください。
リリース時の作業全体は [`RELEASE.md`](programs/RELEASE.md) にまとめています。

### ASP.NET 状態サービスの準備
管理者としてコマンドプロンプトを起動し、下記コマンドを実行します。  
```bat
   sc config aspnet_state start= auto
   net start aspnet_state
```

### サンプルの実行
- 以下のファイルを開いてください。
- web.config または app.config (.NET Coreの場合は、appsettings.json) を開き、  
実際のデータベース環境に合わせて connectionString セクションの値を修正してください。
- サンプルアプリケーションを実行してください。  
ログイン画面が出た場合は、任意の英数字を入力してください。(既定ではパスワード認証を行っていません)  
   
#### Web の場合：
- ASP.NET Web Forms  
  - C:\root\programs\CS\Samples\WebApp_sample\WebForms_Sample\WebForms_Sample.sln
  - C:\root\programs\VB\Samples\WebApp_sample\WebForms_Sample\WebForms_Sample.sln
- ASP.NET MVC  
  - C:\root\programs\CS\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln
  - C:\root\programs\VB\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln

#### C/S 2階層の場合：
- Windows Forms  
  - C:\root\programs\CS\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
  - C:\root\programs\VB\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
- WPF  
  - C:\root\programs\CS\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln
  - C:\root\programs\VB\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln

#### C/S 3階層の場合：
- Windows Forms  
  - 通常の Windows フォームアプリケーション
    - C:\root\programs\CS\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
    - C:\root\programs\VB\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
  - ClickOnce アプリケーション  
C:\root\programs\CS\Samples\WS_sample\WSClient_sample\WSClientWinCone_sample\WSClientWinCone_sample.sln
- WPF
  - C:\root\programs\CS\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln
  - C:\root\programs\VB\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln

### .NET Core アプリケーション

**VB 版は、現時点で .NET Core 版の提供予定はありません。** 以下はいずれも C# のみです。

#### 基盤:
- C:\root\programs\CS\Frameworks\Infrastructure
- C:\root\programs\CS\Frameworks\Infrastructure\ServiceInterface\ASPNETWebServiceCore

#### ツール:
- C:\root\programs\CS\Frameworks\Tools

#### Sample アプリケーション:
- C:\root\programs\CS\Samples4NetCore


## その他 特記事項

### 著作権、ライセンス
[License](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)ディレクトリをご確認ください。

### バグ対応
ご利用いただく中で、バグを発見されましたら、[issue](https://github.com/OpenTouryoProject/OpenTouryo/issues) としてご連絡ください。  
コミュニティで内容を確認し、適切に対応いたします。

### ライブラリの入手、輸出手続き、使用許諾への添付について
- NuGetまたはnpmなどのパッケージ・マネージャーから取得できるライブラリは、Open 棟梁に同梱されないため、輸出管理する必要はありません。
- これ以外のライブラリ、つまりパッケージ・マネージャーから入手できないライブラリは、必要に応じて自身で入手・同梱して輸出する必要があります。この場合、Open棟梁のライセンスに、使用するライブラリのライセンスを添付する必要があります。

### 参考資料
Open 棟梁をご利用いただくにあたり、OpenTouryoDocument リポジトリのドキュメントをご利用いただけます。
- [紹介資料](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/0_Introduction)  
Open 棟梁の概要資料 (PowerPoint のスライドなど) をご覧いただけます。
- [利用ガイド](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/1_User_Guide)  
Open 棟梁の仕組みや、各機能の仕様などをご覧いただけます。
- [チュートリアル](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/2_Tutorial)  
Open 棟梁のファーストステップガイドです。
