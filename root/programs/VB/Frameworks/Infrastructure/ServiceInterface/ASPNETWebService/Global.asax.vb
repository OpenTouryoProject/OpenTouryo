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
'* クラス名        ：WebApiApplication
'* クラス日本語名  ：Global.asaxのコード ビハインド
'*
'* 作成者          ：玄人 幸道
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2026/08/17  玄人 幸道         新規作成（#558）
'*                                Startup.vb（OWIN）から Global.asax 方式へ載せ替え。
'*                                C#版（#495 でテンプレート差し替え済み）に合わせた。
'**********************************************************************************

Imports System.Web.Http
Imports System.Web.Mvc
Imports System.Web.Optimization
Imports System.Web.Routing

Namespace ASPNETWebService
    ''' <summary>Global.asaxのコード ビハインド</summary>
    Public Class WebApiApplication
        Inherits System.Web.HttpApplication

        ''' <summary>アプリケーションの開始に関するイベント</summary>
        Protected Sub Application_Start()
            AreaRegistration.RegisterAllAreas()
            GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
            RouteConfig.RegisterRoutes(RouteTable.Routes)
            BundleConfig.RegisterBundles(BundleTable.Bundles)
        End Sub
    End Class
End Namespace
