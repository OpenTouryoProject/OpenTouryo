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

''' <summary>
''' ルート定義に関する指定
''' </summary>
Public Class RouteConfig
    ''' <summary>
    ''' ［ASP.NET MVC］ルート定義を追加するには？［3.5、4、C#、VB］ － ＠IT
    ''' http://www.atmarkit.co.jp/fdotnet/dotnettips/1031aspmvcrouting1/aspmvcrouting1.html
    '''  RegisterRoutesメソッドはアプリケーション起動
    '''  （Startイベント・ハンドラ）のタイミングで呼び出されるメソッドで、
    '''  デフォルトのルート（名前はDefault）を追加している。
    ''' </summary>
    Public Shared Sub RegisterRoutes(routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        ' ルートを追加するには、ルートの集合を表すRouteCollectionオブジェクトから
        ' MapRouteメソッドを呼び出すだけだ。MapRouteメソッドの構文は、次のとおりである。

        ' MapRouteメソッドの構文 
        ' MapRoute(String name, String url [,Object defaults])  
        ' ・ name：ルート名。
        ' ・ url：URIパターン。
        ' ・ defaults：初期値。 

        ' Defaultルートを定義
        routes.MapRoute(name:="Default", url:="{controller}/{action}/{id}", defaults:=New With {
            Key .controller = "Crud1",
            Key .action = "Index",
            Key .id = UrlParameter.[Optional]
        })

        routes.MapRoute(name:="Default2", url:="{controller}/{action}/{id}", defaults:=New With {
            Key .controller = "Crud2",
            Key .action = "Index",
            Key .id = UrlParameter.[Optional]
        })
    End Sub
End Class