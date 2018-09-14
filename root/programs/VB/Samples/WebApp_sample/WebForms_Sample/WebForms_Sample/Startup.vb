'**********************************************************************************
'* テンプレート
'**********************************************************************************

' サンプル中のテンプレートなので、必要に応じて使用して下さい。

'**********************************************************************************
'* クラス名        ：OwinStartup
'* クラス日本語名  ：OwinStartup
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.Net.Http
Imports System.Web.Optimization
Imports System.Web.Routing

Imports Owin
Imports Microsoft.Owin

Imports Touryo.Infrastructure.Framework.Authentication

<Assembly: OwinStartup(GetType(WebForms_Sample.Startup))>

Namespace WebForms_Sample
    Public Class Startup
        Public Sub Configuration(app As IAppBuilder)
            ' アプリケーションの設定方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=316888 を参照してください

            ' アプリケーションのスタートアップで実行するコードです

            ' URLルーティングの登録
            RouteConfig.RegisterRoutes(RouteTable.Routes)
            ' バンドル＆ミニフィケーションの登録
            BundleConfig.RegisterBundles(BundleTable.Bundles)

            ' JwkSet取得用
            OAuth2AndOIDCClient.HttpClient = New HttpClient()
        End Sub
    End Class
End Namespace
