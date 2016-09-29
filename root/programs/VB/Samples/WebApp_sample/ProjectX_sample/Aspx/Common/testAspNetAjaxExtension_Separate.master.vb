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
'* クラス名        ：Aspx_Common_testAspNetAjaxExtension_Separate
'* クラス日本語名  ：Ajaxテスト用のマスタ ページ（updateパネルを親・子で２つ別けて使用）
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System
Imports Touryo.Infrastructure.Framework.Presentation

''' <summary>Ajaxテスト用のマスタ ページ（updateパネルを親・子で２つ別けて使用）</summary>
Public Partial Class Aspx_Common_testAspNetAjaxExtension_Separate
    Inherits BaseMasterController
    ''' <summary>btnMButton1のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Public Function UOC_btnMButton1_Click(fxEventArgs As FxEventArgs) As String
        ' テキストボックスの値を変更
        Me.TextBox1.Text = "ajaxのポストバック（ボタンクリック）"

        ' ajaxのイベントハンドラでは画面遷移しないこと。
        Return ""
    End Function

    ''' <summary>btnMButton2のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Public Function UOC_btnMButton2_Click(fxEventArgs As FxEventArgs) As String
        ' テキストボックスの値を変更
        Me.TextBox2.Text = "通常のポストバック（ボタンクリック）"

        Return ""
    End Function

    ''' <summary>
    ''' ddlMDropDownList1のSelectedIndexChangedイベント
    ''' </summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Public Function UOC_ddlMDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
        ' テキストボックスの値を変更
        Me.TextBox3.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）"

        ' ajaxのイベントハンドラでは画面遷移しないこと。
        Return ""
    End Function

    ''' <summary>
    ''' ddlMDropDownList2のSelectedIndexChangedイベント
    ''' </summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Public Function UOC_ddlMDropDownList2_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
        ' テキストボックスの値を変更
        Me.TextBox4.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）"

        Return ""
    End Function

    ''' <summary>btnMButton3のクリックイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Public Function UOC_btnMButton3_Click(fxEventArgs As FxEventArgs) As String
        Throw New Exception("Ajaxでエラー")

        'return "";
    End Function
End Class


