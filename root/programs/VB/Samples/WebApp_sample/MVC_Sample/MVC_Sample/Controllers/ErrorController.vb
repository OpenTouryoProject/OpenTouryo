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
'* クラス名        ：ErrorController
'* クラス日本語名  ：エラー処理用コントローラ
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/08/27  Supragyan         Created ErrorController class to display error messages and informations
'*  2015/09/03  Supragyan         Rename Position data class to Exception data class
'*  2015/09/03  Supragyan         Modified Index Action method
'*  2015/09/04  Supragyan         Modified ArrayList to List of ExceptionData on Index action method
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Str

Namespace Controllers
#Region "ErrorController"
    ''' <summary>
    ''' ErrorController class
    ''' </summary>
    Public Class ErrorController
        Inherits Controller
        ''' <summary>Form情報</summary>
        Private list_form As New List(Of PositionData)()

        ''' <summary>Session情報</summary>
        Private list_session As New List(Of PositionData)()

#Region "Index"
        ''' <summary>
        ''' Index Action method to display an error message error information on the screen
        ''' </summary>
        ''' <returns>ActionResult</returns>
        Public Function Index() As ActionResult
            '画面にエラーメッセージ・エラー情報を表示する-----------------------------

            ' To get an error message from Session
            Dim err_msg As String = DirectCast(Session(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE), String)

            ' To get an error information from Session
            Dim err_info As String = DirectCast(Session(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION), String)

            ' Remove exception information from Session
            Session.Remove(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE)
            Session.Remove(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION)

            ' To encode error message and display on Error screen
            ViewBag.label1Data = CustomEncode.HtmlEncode(err_msg)

            ' To encode error information and display on Error screen
            ViewBag.label2Data = CustomEncode.HtmlEncode(err_info)

            Dim sessionAbandonFlag As Boolean = CBool(Session(FxHttpContextIndex.SESSION_ABANDON_FLAG))
            Session.Remove(FxHttpContextIndex.SESSION_ABANDON_FLAG)

            ' ------------------------------------------------------------------------

            '画面にフォーム情報を表示する---------------------------------------------
            Dim form As NameValueCollection = DirectCast(Session(FxHttpContextIndex.FORMS_INFORMATION), NameValueCollection)
            Session.Remove(FxHttpContextIndex.FORMS_INFORMATION)

            If form IsNot Nothing Then
                'foreach
                For Each strKey As String In form
                    If form(strKey) Is Nothing Then
                        'Add key and value to PositionData
                        list_form.Add(New PositionData(strKey, "null"))
                    Else
                        'Add key and value to PositionData
                        list_form.Add(New PositionData(strKey, CustomEncode.HtmlEncode(form(strKey).ToString())))
                    End If
                Next
                'データバインド
                ViewBag.list_form = list_form
            End If

            ' 画面にセッション情報を表示する------------------------------------------

            If Session IsNot Nothing Then
                'foreach
                For Each strKey As String In Session
                    If Session(strKey) Is Nothing Then
                        'Add key and value to PositionData
                        list_session.Add(New PositionData(strKey, "null"))
                    Else
                        'Add key and value to PositionData
                        list_session.Add(New PositionData(strKey, CustomEncode.HtmlEncode(Session(strKey).ToString())))
                    End If
                Next
                'データバインド
                ViewBag.list_session = list_session
            End If

            ' セッション情報を削除する------------------------------------------------

            If sessionAbandonFlag Then
                ' セッション タイムアウト検出用Cookieを消去
                ' ※ Removeが正常に動作しないため、値を空文字に設定 ＝ 消去とする

                ' Set-Cookie HTTPヘッダをレスポンス
                Response.Cookies.[Set](FxCmnFunction.DeleteCookieForSessionTimeoutDetection())

                Try
                    ' セッションを消去                       
                    Session.Abandon()
                Catch ex As Exception
                    ' エラー発生時
                    ' このカバレージを通過する場合、
                    ' おそらく起動した画面のパスが間違っている。
                    Console.WriteLine("このカバレージを通過する場合、おそらく起動した画面のパスが間違っている。")
                    Console.WriteLine(ex.Message)
                End Try
            End If

            Return View()
        End Function

#End Region
    End Class

#End Region

#Region "PositionData"

    ''' <summary>
    ''' ExceptionData class to set key and value for throwing exception 
    ''' </summary>
    Public Class PositionData
        ''' <summary>キー</summary>
        Private _key As String

        ''' <summary>値</summary>
        Private _value As String

        ''' <summary>コンストラクタ</summary>
        Public Sub New(key As String, value As String)
            Me._key = key
            Me._value = value
        End Sub

        ''' <summary>キー</summary>
        Public ReadOnly Property key() As String
            Get
                Return _key
            End Get
        End Property

        ''' <summary>値</summary>
        Public ReadOnly Property value() As String
            Get
                Return _value
            End Get
        End Property
    End Class

#End Region
End Namespace

