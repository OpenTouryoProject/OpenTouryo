# Open 棟梁
 
## 概要 
Open 棟梁は、.NET Framework と .NET Core をベースとしたアプリケーション フレームワークです。

このファイルの英語版は[こちら](README.md)から。

## サンプルの実行手順
Open 棟梁に同梱されるサンプルアプリケーションの実行手順は以下のとおりです。



### 前提ツールのインストール
- あらかじめ、Visual Studio （若しくは Visual Studio Code と .NET系のSDKとExtension）をインストールしておいてください。  
- また、Open 棟梁がサポートしている DBMS 中から使用するものを準備してください。https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker が便利です。
- サポートしている データプロバイダは、https://github.com/OpenTouryoProject/OpenTouryo/tree/develop/root/programs/CS/Frameworks/Infrastructure/Public/Db から確認できます。
- 正式サポートされているデータプロバイダは「SQL/OLE/ODBC/ODP/MCN/NPG」です。現在は除外されていますが「DB2/HiRDB/OracleClient」は雛形があるのでエージェントなどを活用してセルフ・サポートすることは可能です。

-以下の名前空間を持つADO.NETデータプロバイダ
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

#### LocalServicesOnDockerを活用
https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker が便利です。

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

- 「*」エージェントによるビルドシステム解説

- Open 棟梁 が利用するライブラリは、NuGet 経由でダウンロードします。このため、プロキシ環境では、正常に NuGet ライブラリがダウンロードできないことがあります。プロキシ環境をお使いの場合は、以下のように http_proxy 環境変数を定義してください。
    - C:\root\programs\CS\z_Common.bat および C:\root\programs\VB\z_Common.bat を、テキストエディタで開きます。
    - 既定では、http_proxy 環境変数の定義部分はコメントアウトされていますので、"@rem" を削除して、このコメントを解除します。
    - http_proxy 環境変数に、お使いのプロキシ情報を設定してください。

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

#### 基盤:
- C#
  - C:\root\programs\CS\Frameworks\Infrastructure
  - C:\root\programs\CS\Frameworks\Infrastructure\ServiceInterface\ASPNETWebServiceCore
- VB  
...

#### ツール:
- C#
  - C:\root\programs\CS\Frameworks\Tools
- VB  
...

#### Sample アプリケーション:
- C#
  - C:\root\programs\CS\Samples4NetCore
- VB  
...


## その他 特記事項

### 著作権、ライセンス
[License](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)ディレクトリをご確認ください。

### バグ対応
ご利用いただく中で、バグを発見されましたら、[issue](https://github.com/OpenTouryoProject/OpenTouryo/issues) としてご連絡ください。  
コミュニティで内容を確認し、適切に対応いたします。

### NuGetパッケージの作成方法
Open 棟梁の NuGetパッケージを作成する方法については、[こちらの記事](https://github.com/OpenTouryoProject/OpenTouryo/wiki/HowToCreateOpenTouryoNuGetPackages.ja)をご参照ください。

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
