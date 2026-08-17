#Region "Apache License"
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：RouteConfig
'* クラス日本語名  ：ルート定義に関する指定（MVC用）
'*
'* 作成者          ：玄人 幸道
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2026/08/17  玄人 幸道         新規作成（#558）C#版に合わせて追加。
'**********************************************************************************

Imports System.Web.Mvc
Imports System.Web.Routing

Namespace ASPNETWebService
    ''' <summary>ルート定義に関する指定（MVC用）</summary>
    Public Class RouteConfig
        ''' <summary>ルートの登録</summary>
        ''' <param name="routes">RouteCollection</param>
        Public Shared Sub RegisterRoutes(routes As RouteCollection)
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

            routes.MapRoute(
                name:="Default",
                url:="{controller}/{action}/{id}",
                defaults:=New With {
                    .controller = "Home",
                    .action = "Index",
                    Key .id = UrlParameter.[Optional]
                })
        End Sub
    End Class
End Namespace
