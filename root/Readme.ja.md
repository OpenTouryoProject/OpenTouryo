# Open 棟梁
 
## 概要 
Open 棟梁は、.NET Framework と .NET Core をベースとしたアプリケーション フレームワークです。  
Open 棟梁のプログラムは、以下のリポジトリで公開しています。
- OpenTouryoTemplates リポジトリ
    - 概要  
    Open 棟梁を使用したアプリケーションの**開発基盤 (テンプレート)** となるもので、  
    Visual Studio のバージョンごとにフォルダが分かれている
    - 想定利用者  
    Open 棟梁を使用してアプリケーションを開発する人
- OpenTouryo リポジトリ (**現在のリポジトリ**)
    - 概要  
    OpenTouryoTemplates リポジトリの**母体**となるもの  
    (Open 棟梁の各機能は、まず OpenTouryo リポジトリで開発され、  
    その後 OpenTouryoTemplates リポジトリに取り込まれる)
    - 想定利用者  
    OSS 開発者

このため、システム開発プロジェクトで Open 棟梁を使用される方は、[OpenTouryoTemplates リポジトリ](https://github.com/OpenTouryoProject/OpenTouryoTemplates)を使用してください。以下、**OSS 開発者向け**に、Open 棟梁の利用方法をご紹介します。

このファイルの英語版は[こちら](README.md)から。

## サンプルの実行手順
Open 棟梁に同梱されるサンプルアプリケーションの実行手順は以下のとおりです。

**Optional 表記:**  
以下、optional 表記のある DBMS とデータプロバイダは、Open 棟梁でサポートされていますが、Open 棟梁のプログラムには含まれていません。そのため、optional 表記のある DBMS とデータプロバイダを使う場合、必要に応じてデータプロバイダを手動でダウンロードし、[Open 棟梁のデータアクセス用のプロジェクト (DamXXX.csproj)](https://github.com/OpenTouryoProject/OpenTouryo/tree/develop/root/programs/CS/Frameworks/Infrastructure/Public/Db) から、当該データプロバイダに参照設定を張り直してください。

### 前提ツールのインストール
あらかじめ、Visual Studio 2015 をインストールしておいてください。  
.NET Standard、.NET Core開発を行う場合は、Visual Studio 2017 をインストールしておいてください。  
参考: https://docs.microsoft.com/ja-jp/dotnet/core/windows-prerequisites

また、Open 棟梁がサポートしている以下の DBMS へのデータアクセスクラスの開発・テストを行う場合は、使用する DBMS をインストールしてください。
- SQL Server  
(SQL Server のバージョンは任意です。また、エディションについては、Express Edition 以外もお使いいただけますが、サンプルアプリケーションに指定する接続文字列を修正する必要がありますので、ご注意ください)
- Oracle Database (Express Edition を含みます)
- IBM DB2 ... optional
- HiRDB ... optional
- MySQL
- PostgreSQL

### Open 棟梁の配置
「root」フォルダを、C ドライブ直下にコピーしてください。C ドライブ直下以外にコピーすると、Windows のファイルパスの最大文字長の制限により、ビルドに失敗することがあります。

### データプロバイダの取得と配置
Open 棟梁が現在サポートしているデータベースと、対応するデータプロバイダは以下のとおりです。

- Oracle
  - Oracle.DataAccess.dll ... optional
  - Oracle.ManagedDataAccess.dll
- IBM DB2
  - IBM.Data.DB2.dll ... optional
- HiRDB
  - x86: pddndp40.dll, pddndpcore40.dll ... optional
  - x64: pddndp40x.dll, pddndpcore40x.dll ... optional
- MySQL
  - MySql.Data.dll
- PostgreSQL
  - Npgsql.dll

### サンプルデータベースのセットアップ
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
Open 棟梁のプログラムをビルドするときは、**初回のみ、MSBuild を使用したビルドバッチファイルを実行**して、プログラムをビルドします。  
これは、Open 棟梁のテンプレート・ベースには、「フレームワーク部分 (ベースクラス１，２)」と「サンプルアプリケーション」がありますが、フレームワーク部分のビルド生成物 (DLL ファイル) を Open 棟梁の既定の置き場にコピーするなどの処理が必要なためです。  
これらの一連のビルドプロセスをまとめたバッチファイルを実行します。

ビルドバッチファイルは、以下のフォルダにあります。
- C:\root\programs\CS  
- C:\root\programs\VB

実行するバッチファイルは、以下の表のとおりです。
なお、以下の表で
- ○: 必ず実行する
- △: アプリケーションの形態によっては、実行する必要がある
- 空白: 実行する必要はない

を表します。

<table Border="1">
<tr>
<Td style="background-color:#9BC2E6;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;font-weight:bold;" colspan="1" rowspan="2">バッチファイル名</td>
<Td style="background-color:#9BC2E6;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;font-weight:bold;" colspan="1" rowspan="2">説明</td>
<Td style="background-color:#9BC2E6;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;font-weight:bold;" colspan="2" rowspan="1">存在</td>
</tr>
<tr>
<Td style="background-color:#9BC2E6;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;font-weight:bold;">C#</td>
<Td style="background-color:#9BC2E6;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;font-weight:bold;">VB</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">0_ExecAllBat.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">以下のファイルを使用してバッチビルドを行う。必要に応じてカスタマイズしてください。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">1_DeleteDir.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">ビルドによってできたフォルダを削除&nbsp;(クリーンアップ)&nbsp;する。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">1_DeleteFile.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">一時ファイルなどを削除&nbsp;(クリーンアップ)&nbsp;する。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">2_Build_NuGet_net45.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.5.2&nbsp;をターゲットとする&nbsp;NuGet&nbsp;パッケージの作成用に、フレームワーク&nbsp;(ベースクラス１,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*1</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">2_Build_NuGet_net46.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.6&nbsp;をターゲットとする&nbsp;NuGet&nbsp;パッケージの作成用に、フレームワーク&nbsp;(ベースクラス１,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*1</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">2_Build_NuGet_net47.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.7&nbsp;をターゲットとする&nbsp;NuGet&nbsp;パッケージの作成用に、フレームワーク&nbsp;(ベースクラス１,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*1</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">2_Build_NuGet_netstd20.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Standard&nbsp;2.0&nbsp;をターゲットとする&nbsp;NuGet&nbsp;パッケージの作成用に、フレームワーク&nbsp;(ベースクラス１,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*1,&nbsp;*3</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_Business_net45.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.5.2&nbsp;をターゲットとする&nbsp;Business名前空間のフレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_Business_net46.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.6&nbsp;をターゲットとする&nbsp;Business名前空間のフレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_Business_net47.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.7&nbsp;をターゲットとする&nbsp;Business名前空間のフレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_BusinessRichClient_net45.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.5.2&nbsp;をターゲットとする&nbsp;Business名前空間のリッチクライアント用フレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*2</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_BusinessRichClient_net46.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.6&nbsp;をターゲットとする&nbsp;Business名前空間のリッチクライアント用フレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*2</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_BusinessRichClient_net47.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Framework&nbsp;4.7&nbsp;をターゲットとするBusiness名前空間のリッチクライアント用フレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*2</td>
</tr>
<tr>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_Business_netcore20.bat</td>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Core&nbsp;2.0&nbsp;をターゲットとする&nbsp;Business名前空間のフレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*3</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">3_Build_Business_netcore30.bat</td>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Core&nbsp;3.0&nbsp;をターゲットとする&nbsp;Business名前空間のフレームワーク&nbsp;(ベースクラス２,&nbsp;ライブラリ部分)&nbsp;をビルドする。</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*3</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">4_Build_CopyAssemblies.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">上記ビルドのプライマリ出力を参照先フォルダにコピーする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">4_Build_Framework_Tool.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースの付属ツールをビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">5_Build_Bat_sample.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースのサンプルアプリ&nbsp;(バッチ)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*4</td>
</tr>
<tr>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">5_Build_BatCore_sample.bat</td>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Coreベースのサンプルアプリ&nbsp;(バッチ)&nbsp;をビルドする。</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*3,&nbsp;*4</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">5_Build_2CS_sample.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースのサンプルアプリ&nbsp;(2&nbsp;層&nbsp;C/S)&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*4</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">6_Build_WSSrv_sample.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースのサンプルアプリ&nbsp;(Web&nbsp;サービス&nbsp;(サーバー側ロジック))&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*4</td>
</tr>
<tr>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">6_Build_WSSrvCore_sample.bat</td>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Coreベースのサンプルアプリ&nbsp;(Web&nbsp;サービス&nbsp;(サーバー側ロジック))&nbsp;をビルドする。</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">7_Build_Framework_WS.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースのフレームワーク&nbsp;(サービスインタフェース部分)&nbsp;をビルドする。&nbsp;</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*4</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">8_Build_WSClntWin_sample.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースのサンプルアプリ&nbsp;(Web&nbsp;サービスクライアント&nbsp;(Windows&nbsp;Forms))&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*5</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">9_Build_WSClntWPF_sample.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">.NET&nbsp;Frameworkベースのサンプルアプリ&nbsp;(Web&nbsp;サービスクライアント&nbsp;(WPF))&nbsp;をビルドする。</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*6</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">10_Build_WebApp_sample.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">サンプルWebアプリ&nbsp;(ASP.NET)&nbsp;をビルドする。&nbsp;</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;" colspan="2" rowspan="1">○*7</td>
</tr>
<tr>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">10_Build_WebAppCore_sample.bat</td>
<Td style="background-color:#F8CBAD;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">サンプルWebアプリ&nbsp;(ASP.NET&nbsp;Core)&nbsp;をビルドする。&nbsp;</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○*3,&nbsp;*4</td>
<Td style="background-color:#F8CBAD;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">99_BuildLibsByBatsAtOtherRepos.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">当該リポジトリのBusiness名前空間を他のリポジトリで利用する際のバッチ（OpenTouryoTemplates&nbsp;-&nbsp;master&nbsp;ブランチ用）</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">99_BuildLibsByBatsAtOtherReposInTimeOfDev.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">当該リポジトリのBusiness名前空間を他のリポジトリで利用する際のバッチ（OpenTouryo&nbsp;-&nbsp;develop&nbsp;ブランチ用）</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">y_Build_TestCode.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">以下のテストコードのバッチビルドと実行を行う。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">y_Build_TestCode_Public.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">以下のPublic名前空間のテストコードのビルドと実行を行う。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">y_Build_TestCode_SecCUI.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">Public.Security名前空間のCUIテストコードのビルドと実行を行う。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">y_Build_TestCode_SecCUI.txt</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">Public.Security名前空間のCUIテストコードのビルドと実行を行う（WSL用）。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">y_Build_TestCode_SecGUI.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">Public.Security名前空間のGUIテストコードのビルドと実行を行う。</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">z_ChangePackages_net45.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">NuGet&nbsp;パッケージの作成時に、packages.config切り替えるバッチ（.NET&nbsp;Framework&nbsp;4.5.2）</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">z_ChangePackages_net46.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">NuGet&nbsp;パッケージの作成時に、packages.config切り替えるバッチ（.NET&nbsp;Framework&nbsp;4.6）</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">z_ChangePackages_net47.bat</td>
<Td style="background-color:#C6E0B4;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">NuGet&nbsp;パッケージの作成時に、packages.config切り替えるバッチ（.NET&nbsp;Framework&nbsp;4.7）</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#C6E0B4;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">－</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">z_Common.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">共通設定&nbsp;(MSBuild&nbsp;用)</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
</tr>
<tr>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">z_Common2.bat</td>
<Td style="background-color:#FFFFFF;text-align:left;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">共通設定&nbsp;(Visual&nbsp;Studio&nbsp;用)&nbsp;</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
<Td style="background-color:#FFFFFF;text-align:center;color:#000000;font-family:'ＭＳ Ｐゴシック';font-size:11pt;">○</td>
</tr>
</table>

<div style="font-size: small">
  <span style="color: red;">*1</span>　NuGetパッケージを作成を作成する場合は必須<br />
  <span style="color: red;">*2</span>　リッチクライアント アプリケーションを作成する場合は必須<br />
  <span style="color: red;">*3</span>　.NET Standard、.NET Core ベースのアプリケーションを開発する場合は必須<br />
  <span style="color: red;">*4</span>　実際のアプリケーションの形態に応じて選択してください
</div>

上の表を参考に、ビルドバッチファイルを番号順に実行してプログラムをビルドしてください。  

- 必要であれば、環境に合わせて、z_Common.bat 内の BUILDFILEPATH を書き換えてください。  

- Open 棟梁 が利用するライブラリは、NuGet 経由でダウンロードします。このため、プロキシ環境では、正常に NuGet ライブラリがダウンロードできないことがあります。プロキシ環境をお使いの場合は、以下のように http_proxy 環境変数を定義してください。
    - C:\root\programs\CS\z_Common.bat および C:\root\programs\VB\z_Common.bat を、テキストエディタで開きます。
    - 既定では、http_proxy 環境変数の定義部分はコメントアウトされていますので、"@rem" を削除して、このコメントを解除します。
    - http_proxy 環境変数に、お使いのプロキシ情報を設定してください。

- ビルド時に以下のエラーが発生した場合は、Windows 8 用の Windows SDK をインストールしてください。([Open 棟梁の issue](https://github.com/OpenTouryoProject/OpenTouryoTemplates/issues/48#issuecomment-241349223) が参考になります。)
```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\Microsoft.Common.targets(2863,5): error MSB3086: タスクは SdkToolsPath "" またはレジストリ キー "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86" を使用して "AL.exe"を見つけられませんでした。SdkToolsPath が設定されていること、SdkToolsPath の下の適切なプロセッサ固有の場所にツールが存在すること、および Microsoft Windows SDK がインストールされていることを確認してください。 [C:\root\programs\CS\Frameworks\Infrastructure\Public\Public.csproj]
```
  
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
- ASP.NET Core MVC  
  - C:\root\programs\CS\Samples4NetCore\WebApp_sample\MVC_Sample\MVC_Sample.sln

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
