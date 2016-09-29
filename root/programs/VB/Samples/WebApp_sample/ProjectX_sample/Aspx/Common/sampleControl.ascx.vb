'**********************************************************************************
'* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
'**********************************************************************************

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
'* クラス名        ：Aspx_Common_WebUserControl
'* クラス日本語名  ：Aspx_Common_WebUserControl上のイベントハンドラをハンドルする。
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections

' System.Web
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' フレームワーク
Imports Touryo.Infrastructure.Framework.Presentation

''' <summary>WebUserControl class</summary>
Public Partial Class Aspx_Common_WebUserControl
    Inherits System.Web.UI.UserControl
    ''' <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnUCButton_Click(fxEventArgs As FxEventArgs) As String
        Response.Write("UOC_btnUCButton_Clickを実行できた。")

        Return ""
    End Function
End Class

