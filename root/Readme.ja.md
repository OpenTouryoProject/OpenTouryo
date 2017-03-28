# Open 棟梁
Open 棟梁は、.NET Framework をベースとしたアプリケーション フレームワークです。  
Open 棟梁のプログラムは、以下のリポジトリで公開しています。
- OpenTouryoTemplates リポジトリ
    - 概要  
    Open 棟梁を使用したアプリケーションの**開発基盤 (テンプレート)** となるもの  
    SQL Server を既定の DBMS とし、Visual Studio のバージョンごとにフォルダが分かれている
    - 想定利用者  
    Open 棟梁を使用してアプリケーションを開発する人
- OpenTouryo リポジトリ (**現在のリポジトリ**)
    - 概要  
    OpenTouryoTemplates リポジトリの**母体**となるもの  
    (Open 棟梁の各機能は、まず OpenTouryo リポジトリで開発され、その後 OpenTouryoTemplates リポジトリに取り込まれる)
    - 想定利用者  
    OSS 開発者

このため、システム開発プロジェクトで Open 棟梁を使用される方は、[OpenTouryoTemplates リポジトリ](https://github.com/OpenTouryoProject/OpenTouryoTemplates)を使用してください。  
以下、**OSS 開発者向け**に、Open 棟梁の利用方法をご紹介します。  

このファイルの英語版は[こちら](README.md)から。

## サンプルの実行手順
Open 棟梁に同梱されるサンプルアプリケーションの実行手順は以下のとおりです。

### 前提ツールのインストール
あらかじめ、Visual Studio 2015 をインストールしておいてください。

また、Open 棟梁がサポートしている以下の DBMS へのデータアクセスクラスの開発・テストを行う場合は、使用する DBMS をインストールしてください。
- SQL Server  
(SQL Server のバージョンは任意です。また、エディションについては、Express Edition 以外もお使いいただけますが、サンプルアプリケーションに指定する接続文字列を修正する必要がありますので、ご注意ください)
- Oracle Database (Express Edition を含みます)
- IBM DB2
- MySQL
- PostgreSQL

### Open 棟梁の配置
「root」フォルダを、C ドライブ直下にコピーしてください。  

### データプロバイダの配置
Open 棟梁がサポートするデータベースのデータプロバイダを入手し、以下のフォルダに配置します。
```txt
C:\root\programs\C#\Frameworks\Infrastructure\Public\Dll
```

Open 棟梁が現在サポートしているデータベースと、対応するデータプロバイダは以下のとおりです。
- Oracle (Oracle.DataAccess.dll)
- DB2 (IBM.Data.DB2.dll)
- MySQL (MySql.Data.dll)
- PostgreSQL (Npgsql.dll)

**注意:**  
Open 棟梁は SQL Server もサポートしていますが、SQL Server のデータプロバイダである System.Data.SqlClient.dll は .NET Framework に同梱されていますので、上記フォルダに格納する必要はありません。  
また、Open 棟梁のデータアクセス用のプロジェクト (DamXXX.csproj) から、各データプロバイダに参照設定をはりなおす必要がある場合があります。

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
- C:\root\files\resource\Sql\[DBMS 名]\TestTable.txt を実行し、テスト用のテーブルを作成してください。

### プログラムのビルド
Open 棟梁のプログラムをビルドするときは、**初回のみ、MSBuild を使用したビルドバッチファイルを実行**して、プログラムをビルドします。  
これは、Open 棟梁のテンプレートベースには、「フレームワーク部分 (ベースクラス１，２)」と「サンプルアプリケーション」がありますが、フレームワーク部分のビルド生成物 (DLL ファイル) を Open 棟梁の既定の置き場にコピーするなどの処理が必要なためです。  
これらの一連のビルドプロセスをまとめたバッチファイルを実行します。

ビルドバッチファイルは、以下のフォルダにあります。
- C:\root\programs\C#  
- C:\root\programs\VB

実行するバッチファイルは、以下の表のとおりです。
なお、以下の表で
- ○: 必ず実行する
- △: アプリケーションの形態によっては、実行する必要がある
- 空白: 実行する必要はない

を表します。

<table>
  <thead>
    <tr>
      <th rowspan="2">フォルダ名</th><th rowspan="2">バッチファイル名</th><th rowspan="2">説明</th><th colspan="2">実行するファイル</th>
    </tr>
    <tr>
      <th>C#</th><th>VB</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td rowspan="16" style="vertical-align: top">C:\root\programs\C#</td><td>1_DeleteDir.bat</td><td>ビルドによってできたフォルダを削除 (クリーン) する。</td><td>○</td><td>○</td>
    </tr>
    <tr>
      <td>2_DeleteFile.bat</td><td>一時ファイルなどを削除 (クリーン) する。</td><td>○</td><td>○</td>
    </tr>
    <tr>
      <td>3_Build_Framework.bat</td><td>フレームワーク部分 (ベースクラス１，２) をビルドする。</td><td>○</td><td>○</td>
    </tr>
    <tr>
      <td>3_Build_PortableClassLibrary.bat</td><td>ポータブルクラスライブラリをビルドする。</td><td>△<span style="color: red"><sup>*1</sup></span></td><td></td>
    </tr>
    <tr>
      <td>3_Build_RichClientCustomControl.bat</td><td>リッチクライアント用カスタムコントロールをビルドする。</td><td>△<span style="color: red"><sup>*2</sup></span></td><td></td>
    </tr>
    <tr>
      <td>4_Build_Framework_Tool.bat</td><td>付属ツールをビルドする。</td><td>○</td><td>○</td>
    </tr>
    <tr>
      <td>5_Build_2CS_sample.bat</td><td>サンプルアプリ (2 層 C/S) をビルドする。</td><td rowspan="8" style="vertical-align: top">△<span style="color: red"><sup>*3</sup></span></td><td></td>
    </tr>
    <tr>
      <td>5_Build_Bat_sample.bat</td><td>サンプルアプリ (バッチ) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>6_Build_WSSrv_sample.bat</td><td>サンプルアプリ (Web サービス (サーバー側ロジック)) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>7_Build_Framework_WS.bat</td><td>フレームワーク (サービスインタフェース部分) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>8_Build_WSClnt_sample.bat</td><td>サンプルアプリ (Web サービスクライアント (Windows フォーム)) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>9_Build_WSClnt_sample.bat</td><td>サンプルアプリ (Web サービスクライアント (WPF)) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>10_Build_WebApp_sample.bat</td><td>サンプルアプリ (ASP.NET) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>11_Build_UWP_sample.bat</td><td>サンプルアプリ (UWP) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>z_Common.bat</td><td>共通設定 (MSBuild 用)</td><td></td><td></td>
    </tr>
    <tr>
      <td>z_Common2.bat</td><td>共通設定 (Visual Studio 用)</td><td></td><td></td>
    </tr>
    <tr>
      <td rowspan="12" style="vertical-align: top">C:\root\programs\VB</td><td>1_DeleteDir.bat</td><td>ビルドによってできたフォルダを削除 (クリーン) する。</td><td></td><td>○</td>
    </tr>
    <tr>
      <td>2_DeleteFile.bat</td><td>一時ファイルなどを削除 (クリーン) する。</td><td></td><td>○</td>
    </tr>
    <tr>
      <td>3_Build_Framework.bat</td><td>フレームワーク (ベースクラス２) をビルドする。</td><td></td><td>○</td>
    </tr>
    <tr>
      <td>3_Build_RichClientCustomControl.bat</td><td>リッチクライアント用カスタムコントロールをビルドする。</td><td></td><td>△<span style="color: red"><sup>*2</sup></span></td>
    </tr>
    <tr>
      <td>5_Build_2CS_sample.bat</td><td>サンプルアプリ (2 層 C/S) をビルドする。</td><td></td><td rowspan="6" style="vertical-align: top">△<span style="color: red"><sup>*3</sup></span></td>
    </tr>
    <tr>
      <td>5_Build_Bat_sample.bat</td><td>サンプルアプリ (バッチ) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>6_Build_WSSrv_sample.bat</td><td>サンプルアプリ (Web サービス (サーバー側ロジック)) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>7_Build_Framework_WS.bat</td><td>フレームワーク (サービスインタフェース部分) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>8_Build_WSClnt_sample.bat</td><td>サンプルアプリ (Web サービスクライアント (Windows フォーム)) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>10_Build_WebApp_sample.bat</td><td>サンプルアプリ (ASP.NET) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>z_Common.bat</td><td>共通設定 (MSBuild 用)</td><td></td><td></td>
    </tr>
    <tr>
      <td>z_Common2.bat</td><td>共通設定 (Visual Studio 用)</td><td></td><td></td>
    </tr>
  </tbody>
</table>
<div style="font-size: small">
  <span style="color: red;">*1</span>　Portable Class Libraryを作成する場合は必須<br />
  <span style="color: red;">*2</span>　リッチクライアント アプリケーションを作成する場合は必須<br />
  <span style="color: red;">*3</span>　実際のアプリケーションの形態に応じて選択してください
</div>

上の表を参考に、ビルドバッチファイルを番号順に実行してプログラムをビルドしてください。  

- 必要であれば、環境に合わせて、z_Common.bat 内の BUILDFILEPATH を書き換えてください。  

- ビルド時に以下のエラーが発生した場合は、Windows SDK をインストールしてください。([Open 棟梁の issue](https://github.com/OpenTouryoProject/OpenTouryoTemplates/issues/48#issuecomment-241349223) が参考になります。)  
なお、どのバージョンの Windows 用の Windows SDK を入れたらよいかは、エラーメッセージから判断できます。例えば、以下のメッセージであれば、「"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\\**v8.0A**\WinSDK-NetFx40Tools-x86" を使用して」とありますので、「Windows 8 用の Windows SDK」をインストールしたらよいことが分かります。
```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\Microsoft.Common.targets(2863,5): error MSB3086: タスクは SdkToolsPath "" またはレジストリ キー "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86" を使用して "AL.exe"を見つけられませんでした。SdkToolsPath が設定されていること、SdkToolsPath の下の適切なプロセッサ固有の場所にツールが存在すること、および Microsoft Windows SDK がインストールされていることを確認してください。 [C:\root\programs\C#\Frameworks\Infrastructure\Public\Public.csproj]
```
  
### ASP.NET 状態サービスの準備
管理者としてコマンドプロンプトを起動し、下記コマンドを実行します。  
```bat
   sc config aspnet_state start= auto
   net start aspnet_state
```

### サンプルの実行
- 以下のファイルを開いてください。
- web.config または app.config を開き、実際のデータベース環境に合わせて connectionString セクションの値を修正してください。
- サンプルアプリケーションを実行してください。  
ログイン画面が出た場合は、任意の英数字を入力してください。(既定ではパスワード認証を行っていません)  
   
#### Web の場合：
- ASP.NET Web Forms  
  - C:\root\programs\C#\Samples\WebApp_sample\ProjectX_sample\ProjectX_sample.sln
  - C:\root\programs\VB\Samples\WebApp_sample\ProjectX_sample\ProjectX_sample.sln
- ASP.NET MVC  
  - C:\root\programs\C#\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln
  - C:\root\programs\VB\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln
- ASP.NET Single Page Application  
C:\root\programs\C#\Samples\WebApp_sample\SPA_Sample\SPA_Sample.sln
 
#### C/S 2階層の場合：
- Windows Forms  
  - C:\root\programs\C#\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
  - C:\root\programs\VB\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
- WPF  
  - C:\root\programs\C#\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln
  - C:\root\programs\VB\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln

#### C/S 3階層の場合：
- Windows Forms  
  - 通常の Windows フォームアプリケーション
    - C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
    - C:\root\programs\VB\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
  - ClickOnce アプリケーション  
C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWinCone_sample\WSClientWinCone_sample.sln
- WPF
  - C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln
  - C:\root\programs\VB\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln
- UWP  
C:\root\programs\C#\Samples\UWP_sample\UWP_sample.sln

### 参考資料
Open 棟梁をご利用いただくにあたり、OpenTouryoDocument リポジトリのドキュメントをご利用いただけます。
- [紹介資料](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/0_Introduction)  
Open 棟梁の概要資料 (PowerPoint のスライドなど) をご覧いただけます。
- [利用ガイド](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/1_User_Guide)  
Open 棟梁の仕組みや、各機能の仕様などをご覧いただけます。
- [チュートリアル](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/2_Tutorial)  
Open 棟梁のファーストステップガイドです。     
   
### 著作権、ライセンス
[License](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)ディレクトリをご確認ください。

### バグ対応
ご利用いただく中で、バグを発見されましたら、[issue](https://github.com/OpenTouryoProject/OpenTouryo/issues) としてご連絡ください。  
コミュニティで内容を確認し、適切に対応いたします。

### データプロバイダの入手、輸出手続き、使用許諾への添付について
Open 棟梁はいろいろなデータプロバイダをサポートしていますが、
各データプロバイダの入手・輸出手続きに関しては、各自でご対応ください。
