# Open 棟梁 UWP サンプル
## 概要
このフォルダーには、Open 棟梁を使った UWP アプリのサンプルがあります。

## ソリューション構造
- UWP_Sample.Html  
HTML/JavaScript で作成した UWP アプリサンプルです。
- UWP_Sample.Xaml  
XAML/C# で作成した UWP アプリサンプルです。
- SPA_sample  
シングルページ アプリケーションと、Web API のサンプルです。UWP アプリサンプルから、このプロジェクトの Web API を呼び出します。

## 使い方
### Windows 10 の開発者モードを有効にする
UWP アプリの開発を行う場合、Windows 10 の開発者機能を使う必要があります。

1. Windows 10 の「更新とセキュリティ」を開きます。
1. 「開発者向け」メニューをクリックし、「開発者モード」を選択してください。

詳しくは、[マイクロソフトのサイト](https://docs.microsoft.com/ja-jp/windows/uwp/get-started/enable-your-device-for-development)をご参照ください。

### アプリを署名する
1. UWP_Sample.Html または UWP_Sample.Xaml プロジェクトの *package.appxmanifest* ファイルを開いてください。
1. 「パッケージ化」を選択し、「証明書の選択」ボタンをクリックしてください。
1. 署名用の証明書を選択してください。テスト証明書を作成することもできます。

### フォーム認証を無効にする
既定では、SPA_sample のフォーム認証が有効になっています。UWP アプリから Web API を呼び出せるように、フォーム認証を無効にしてください。

**Web.config** を開き、`<deny users="?" />` をコメントアウトしてください。

```xml
<authorization>
  <!-- 全ユーザーへの許可 -->
  <!--<allow users="*" />-->
  <!-- 匿名ユーザーの禁止 -->
  <!--<deny users="?" />-->
  <!--  
    <allow  users="[ユーザーのコンマ区切り一覧]"
        roles="[ロールのコンマ区切り一覧]"/>
    <deny  users="[ユーザーのコンマ区切り一覧]"
        roles="[ロールのコンマ区切り一覧]"/>
  -->
</authorization>
```

### UWP アプリをデバッグ実行する
1. UWP_Sample.Html プロジェクトまたは UWP_Sample.Xaml プロジェクトを右クリックし、「スタートアップ プロジェクトに設定」を選択してください。
1. [F5] キーを押し、アプリケーションをデバッグ実行してください。(自動的に IIS Express が起動し、SPA_Sample が実行されます。デバッグなしで実行すると、IIS Express が起動しません)
