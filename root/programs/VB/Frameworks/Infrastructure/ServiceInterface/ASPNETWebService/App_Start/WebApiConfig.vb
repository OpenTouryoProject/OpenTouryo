'**********************************************************************************
'* テンプレート
'**********************************************************************************

' サンプル中のテンプレートなので、必要に応じて使用して下さい。

'**********************************************************************************
'* クラス名        ：WebApiConfig
'* クラス日本語名  ：ルート定義に関する指定（WebApi用）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2026/08/17  玄人 幸道         C#版に合わせた（#558）。C#版は #495 の
'*                                テンプレート差し替えで、この形になっている。
'**********************************************************************************

Imports System.Web.Http

Namespace ASPNETWebService
    Public NotInheritable Class WebApiConfig
        Private Sub New()
        End Sub
        Public Shared Sub Register(config As HttpConfiguration)
            ' Web API の設定およびサービス

            ' Web API ルート
            config.MapHttpAttributeRoutes()

            config.Routes.MapHttpRoute(name:="DefaultApi", routeTemplate:="api/{controller}/{id}", defaults:=New With {
                Key .id = RouteParameter.[Optional]
            })
        End Sub
    End Class
End Namespace
