'**********************************************************************************
'* テンプレート
'**********************************************************************************

' 以下のLicenseに従い、このProjectをTemplateとして使用可能です。Release時にCopyright表示してSublicenseして下さい。
' https://github.com/OpenTouryoProject/OpenTouryo/blob/master/license/LicenseForTemplates.txt

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