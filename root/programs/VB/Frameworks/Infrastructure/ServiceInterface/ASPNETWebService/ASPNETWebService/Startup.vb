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

Imports System.Web.Mvc
Imports System.Web.Http
Imports System.Net.Http

Imports Owin
Imports Microsoft.Owin

Imports Touryo.Infrastructure.Framework.Authentication

<Assembly: OwinStartup(GetType(ASPNETWebService.Startup))>

Namespace ASPNETWebService
    Public Class Startup
        ''' <summary>Configuration</summary>
        ''' <param name="app"></param>
        Public Sub Configuration(app As IAppBuilder)
            ' アプリケーションの設定方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=316888 を参照してください

            ' アプリケーションのスタートアップで実行するコードです

            '
            AreaRegistration.RegisterAllAreas()

            ' 
            WebApiConfig.Register(GlobalConfiguration.Configuration)

            ' グローバルフィルタの登録
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)

            '' URLルーティングの登録
            'RouteConfig.RegisterRoutes(RouteTable.Routes)

            '' バンドル＆ミニフィケーションの登録
            'BundleConfig.RegisterBundles(BundleTable.Bundles)

            '/ 認証に関するOWINミドルウェアの設定を行う。
            'StartupAuth.Configure(app);
            OAuth2AndOIDCClient.HttpClient = New HttpClient() ' JwkSet取得用

            GlobalConfiguration.Configuration.EnsureInitialized()
            'GlobalConfiguration.Configuration.Initializer(GlobalConfiguration.Configuration) ??
        End Sub
    End Class
End Namespace
