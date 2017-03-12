'**********************************************************************************
'* テンプレート
'**********************************************************************************

' 以下のLicenseに従い、このProjectをTemplateとして使用可能です。Release時にCopyright表示してSublicenseして下さい。
' https://github.com/OpenTouryoProject/OpenTouryo/blob/master/license/LicenseForTemplates.txt

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

Imports Owin
Imports Microsoft.Owin

Imports System.Web.Optimization
Imports System.Web.Routing

<Assembly: OwinStartup(GetType(ProjectX_sample.Startup))>

Namespace ProjectX_sample
    Public Class Startup
        Public Sub Configuration(app As IAppBuilder)
            ' アプリケーションの設定方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=316888 を参照してください

            ' アプリケーションのスタートアップで実行するコードです

            ' URLルーティングの登録
            RouteConfig.RegisterRoutes(RouteTable.Routes)
            ' バンドル＆ミニフィケーションの登録
            BundleConfig.RegisterBundles(BundleTable.Bundles)
        End Sub
    End Class
End Namespace
