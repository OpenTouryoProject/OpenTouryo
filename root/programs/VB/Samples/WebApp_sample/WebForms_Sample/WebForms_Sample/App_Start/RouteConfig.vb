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

Imports System.Web.Routing
'using Microsoft.AspNet.FriendlyUrls;

Public NotInheritable Class RouteConfig
    Private Sub New()
    End Sub
    Public Shared Sub RegisterRoutes(routes As RouteCollection)
        'var settings = new FriendlyUrlSettings();
        'settings.AutoRedirectMode = RedirectMode.Permanent;
        'routes.EnableFriendlyUrls(settings);
    End Sub
End Class