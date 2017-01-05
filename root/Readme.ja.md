# Open 棟梁テンプレート・ベース (Visual Studio 2015 用)
「Open 棟梁テンプレート・ベース」とは、Open 棟梁を使用したシステムの開発基盤 (プロジェクトテンプレート) を作るための元になるものです。

このファイルの英語版は[こちら](Readme.md)から。

### サンプルの実行手順
Open 棟梁テンプレート・ベース (Visual Studio 2015 用) に同梱されるサンプルアプリケーションの実行手順は以下のとおりです。

##### 前提ツールのインストール
あらかじめ、Visual Studio 2015 と SQL Server Express をインストールしておいてください。  
(SQL Server のバージョンは任意です。また、エディションについては、Express Edition 以外もお使いいただけますが、サンプルアプリケーションに指定する接続文字列を修正する必要がありますので、ご注意ください)
   
##### サンプルデータベースのセットアップ
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

##### テンプレート・ベースの配置
「root_VS2015」フォルダを、C ドライブ直下にコピーしてください。  
そして、フォルダ名を「root_VS2015」から「root」にリネームしてください。

##### プログラムのビルド
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
      <td rowspan="17" style="vertical-align: top">C:\root\programs\C#</td><td>1_DeleteDir.bat</td><td>ビルドによってできたフォルダを削除 (クリーン) する。</td><td>○</td><td>○</td>
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
      <td>3_Build_RichClientCustomControl.bat</td><td>リッチクライアント用カスタムコントロールをビルドする。</td><td>△<span style="color: red"><sup>*2</sup></span></td><td>△<span style="color: red"><sup>*2</sup></span></td>
    </tr>
    <tr>
      <td>4_Build_Framework_Tool.bat</td><td>付属ツールをビルドする。</td><td>○</td><td>○</td>
    </tr>
    <tr>
      <td>5_Build_2CS_sample.bat</td><td>サンプルアプリ (2 層 C/S) をビルドする。</td><td rowspan="9" style="vertical-align: top">△<span style="color: red"><sup>*3</sup></span></td><td></td>
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
      <td>11_Build_WinAzure_sample.bat</td><td>サンプルアプリ (Azure PaaS) をビルドする。</td><td></td>
    </tr>
    <tr>
      <td>12_Build_WinStore_sample.bat</td><td>サンプルアプリ (Windows ストアアプリ) をビルドする。</td><td></td>
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
  <span style="color: red;">*1</span>　Windows ストアアプリを作成する場合は必須<br />
  <span style="color: red;">*2</span>　リッチクライアント アプリケーションを作成する場合は必須<br />
  <span style="color: red;">*3</span>　実際のアプリケーションの形態に応じて選択してください
</div>

上の表を参考に、ビルドバッチファイルを番号順に実行してプログラムをビルドしてください。  

- 必要であれば、環境に合わせて、z_Common.bat 内の BUILDFILEPATH を書き換えてください。  

- Open 棟梁 Visual Studio 2015 テンプレート・ベースが利用するライブラリは、NuGet 経由でダウンロードします。このため、プロキシ環境では、正常に NuGet ライブラリがダウンロードできないことがあります。プロキシ環境をお使いの場合は、以下のように http_proxy 環境変数を定義してください。
    - C:\root\programs\C#\z_Common.bat および C:\root\programs\VB\z_Common.bat を、テキストエディタで開きます。
    - 既定では、http_proxy 環境変数の定義部分はコメントアウトされていますので、"@rem" を削除して、このコメントを解除します。
    - http_proxy 環境変数に、お使いのプロキシ情報を設定してください。

- ビルド時に以下のエラーが発生した場合は、Windows 8 用の Windows SDK をインストールしてください。([Open 棟梁の issue](https://github.com/OpenTouryoProject/OpenTouryoTemplates/issues/48#issuecomment-241349223) が参考になります。)
```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\Microsoft.Common.targets(2863,5): error MSB3086: タスクは SdkToolsPath "" またはレジストリ キー "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86" を使用して "AL.exe"を見つけられませんでした。SdkToolsPath が設定されていること、SdkToolsPath の下の適切なプロセッサ固有の場所にツールが存在すること、および Microsoft Windows SDK がインストールされていることを確認してください。 [C:\root\programs\C#\Frameworks\Infrastructure\Public\Public.csproj]
```
  
##### ASP.NET 状態サービスの準備
管理者としてコマンドプロンプトを起動し、下記コマンドを実行します。  
```bat
   sc config aspnet_state start= auto
   net start aspnet_state
```

##### Web サービスの URL の変更
このテンプレート・ベース内の Web サービスは、以下の環境で実行することを想定しています。
- 開発時 (デバッグ時) : IIS Express
- 運用時 : IIS

上記の環境で実行した場合、以下のように URL が異なります。
```txt
IIS Express 使用時
http://localhost:xxxx/Service.asmx (xxxx: ポート番号)

IIS 使用時
http://localhost/yyyy/Service.asmx (yyyy: IIS のアプリケーション名)
```

この Web サービスの URL は、Open 棟梁の通信制御機能の設定ファイルに定義されます。
既定では、IIS Express 使用時の URL が使用されます。  
このため、Web サービスを IIS にデプロイした場合、以下の変更が必要になります。

- FxXMLTMProtocolDefinition プロパティの値を「TMProtocolDefinition2.xml」から「TMProtocolDefinition.xml」に変更する。  
- WinStore_sample の baseUrl プロパティを変更する
  - *.htmlファイルの中
  - App.xamlファイルの中

##### サンプルの実行
以下のファイルを開き、実行してください。  
ログイン画面が出た場合は、任意の英数字を入力してください。(既定ではパスワード認証を行っていません)  
   
###### Web の場合：
- ASP.NET  
C:\root\programs\C#\Samples\WebApp_sample\ProjectX_sample\ProjectX_sample.sln
- ASP.NET MVC  
C:\\root\programs\C#\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln
- ASP.NET シングルページ アプリケーション
C:\\root\programs\C#\Samples\WebApp_sample\SPA_Sample\SPA_Sample.sln
- Microsoft Azure 上の PaaS アプリケーション  
C:\root\programs\C#\Samples\WinAzure_sample\WinAzure_sample.sln
 
###### C/S 2階層の場合：
- Windows Forms  
C:\root\programs\C#\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
- WPF  
C:\root\programs\C#\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln

###### C/S 3階層の場合：
- Windows Forms  
  - 通常の Windows フォームアプリケーション  
  C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
  - ClickOnce アプリケーション  
  C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWinCone_sample\WSClientWinCone_sample.sln
- WPF  
C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln
- Windows ストアアプリ  
C:\root\programs\C#\Samples\WinStore_samples\WinStore_samples.sln

##### 参考資料
Open 棟梁をご利用いただくにあたり、OpenTouryoDocument リポジトリのドキュメントをご利用いただけます。
- [紹介資料](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/0_Introduction)  
Open 棟梁の概要資料 (PowerPoint のスライドなど) をご覧いただけます。
- [利用ガイド](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/1_User_Guide)  
Open 棟梁の仕組みや、各機能の仕様などをご覧いただけます。
- [チュートリアル](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/2_Tutorial)  
Open 棟梁のファーストステップガイドです。     
   
### テンプレート・ベースのカスタマイズ
もし Open 棟梁の機能の中で、システム開発プロジェクトの要件に合わない部分がありましたら、このテンプレート・ベースをカスタマイズすることでご対応いただけます。  
テンプレートベースのカスタマイズ方法につきましては、[チュートリアル](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/2_Tutorial/ja-JP/Tutorial_Template_development.doc)をご覧ください。
   
### 著作権、ライセンス
[License](https://github.com/OpenTouryoProject/OpenTouryoTemplates/tree/master/license)ディレクトリをご確認ください。

### バグ対応
ご利用いただく中で、バグを発見されましたら、[issue](https://github.com/OpenTouryoProject/OpenTouryoTemplates/issues) としてご連絡ください。  
コミュニティで内容を確認し、適切に対応いたします。

バグ修正パッチは、最新版のテンプレート・ベースを取込むことで適用できます。
もしくは、当該バグをトラッキングツール上から確認して、リポジトリからバグフィックス時の DIFF を取得し、マージしてください。

### データプロバイダの入手、輸出手続き、使用許諾への添付について
Open 棟梁はいろいろなデータプロバイダをサポートしていますが、
各データプロバイダの入手・輸出手続きに関しては、各自でご対応ください。
