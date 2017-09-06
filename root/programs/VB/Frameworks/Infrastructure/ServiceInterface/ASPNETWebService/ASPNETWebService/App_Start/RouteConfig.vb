'**********************************************************************************
'* テンプレート
'**********************************************************************************

' サンプル中のテンプレートなので、必要に応じて使用して下さい。

'**********************************************************************************
'* クラス名        ：RouteConfig
'* クラス日本語名  ：ルート定義に関する指定
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Routing
Imports Microsoft.AspNet.FriendlyUrls

Namespace ASPNETWebService
    Public NotInheritable Class RouteConfig
        Private Sub New()
        End Sub
        Public Shared Sub RegisterRoutes(routes As RouteCollection)
            Dim settings = New FriendlyUrlSettings()
            settings.AutoRedirectMode = RedirectMode.Permanent
            routes.EnableFriendlyUrls(settings)

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

            routes.MapRoute(name:="Default", url:="{controller}/{action}/{id}", defaults:=New With {
                Key .action = "Index",
                Key .id = UrlParameter.[Optional]
            })
        End Sub
    End Class
End Namespace
