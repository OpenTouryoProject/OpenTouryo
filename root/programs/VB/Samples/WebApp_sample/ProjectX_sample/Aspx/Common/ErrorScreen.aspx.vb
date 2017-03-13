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
'* クラス名        ：ErrorScreen
'* クラス日本語名  ：例外発生時に表示される画面（開発用 ※ 本稼動時には、本番用に差し替える）
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Str

Namespace Aspx.Common
    Partial Public Class ErrorScreen
        Inherits System.Web.UI.Page
        ' リピータ用

        ''' <summary>Form情報：リピータ処理用</summary>
        Private al_form As New ArrayList()

        ''' <summary>Session情報：リピータ処理用</summary>
        Private al_session As New ArrayList()

#Region "Event Handler"

        ''' <summary>
        ''' 画面起動時に実行されるEvent Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Sub Page_Load(sender As Object, e As EventArgs)
            '画面にError Message・エラー情報を表示する-----------------------------

            'Error MessageをＨＴＴＰコンテキストから取得
            Dim err_msg As String = DirectCast(HttpContext.Current.Items(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE), String)

            'エラー情報をＨＴＴＰコンテキストから取得
            Dim err_info As String = DirectCast(HttpContext.Current.Items(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION), String)

            '画面にError Messageを表示する
            Me.Label1.Text = CustomEncode.HtmlEncode(err_msg)

            '画面にエラー情報を表示する
            Me.Label2.Text = CustomEncode.HtmlEncode(err_info)

            ' ------------------------------------------------------------------------

            '画面にフォーム情報を表示する---------------------------------------------

            'ＨＴＴＰリクエスト フォーム情報
            Dim req As HttpRequest = HttpContext.Current.Request

            'コレクション
            Dim froms As System.Collections.Specialized.NameValueCollection = req.Form

            If froms IsNot Nothing Then
                'foreach
                For Each strKey As String In froms
                    If froms(strKey) Is Nothing Then
                        al_form.Add(New PositionData(strKey, "null"))
                    Else
                        al_form.Add(New PositionData(strKey, CustomEncode.HtmlEncode(froms(strKey).ToString())))
                    End If
                Next

                'データバインド
                Me.Repeater1.DataSource = al_form
                Me.Repeater1.DataBind()
            End If

            ' ------------------------------------------------------------------------

            ' 画面にセッション情報を表示する------------------------------------------

            'ＨＴＴＰセッション情報
            Dim sess As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session

            If sess IsNot Nothing Then
                'foreach
                For Each strKey As String In sess
                    If sess(strKey) Is Nothing Then
                        al_session.Add(New PositionData(strKey, "null"))
                    Else
                        al_session.Add(New PositionData(strKey, CustomEncode.HtmlEncode(sess(strKey).ToString())))
                    End If
                Next

                'データバインド
                Me.Repeater2.DataSource = al_session
                Me.Repeater2.DataBind()
            End If

            ' ------------------------------------------------------------------------

            ' セッション情報を削除する------------------------------------------------

            If CBool(HttpContext.Current.Items(FxHttpContextIndex.SESSION_ABANDON_FLAG)) Then
                ' 2009/09/18-start

                ' セッション タイムアウト検出用Cookieを消去
                ' ※ Removeが正常に動作しないため、値を空文字に設定 ＝ 消去とする

                ' Set-Cookie HTTPヘッダをレスポンス
                Response.Cookies.[Set](FxCmnFunction.DeleteCookieForSessionTimeoutDetection())

                ' 2009/09/18-end

                Try
                    ' セッションを消去
                    Session.Abandon()
                Catch ex2 As Exception
                    ' エラー発生時

                    ' このカバレージを通過する場合、
                    ' おそらく起動した画面のパスが間違っている。
                    Console.WriteLine("このカバレージを通過する場合、おそらく起動した画面のパスが間違っている。")
                    Console.WriteLine(ex2.Message)
                End Try
            End If
        End Sub

#End Region

    End Class

#Region "リピータ処理用クラス"

    ''' <summary>リピータ処理用クラス</summary>
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
