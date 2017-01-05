'**********************************************************************************
'* サンプル アプリ・コントローラ
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：ErrorController
'* クラス日本語名  ：Html.BeginForm用サンプル アプリ・コントローラ
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/08/27  Supragyan         Created ErrorController class to display error messages and informations
'*  2015/09/03  Supragyan         Rename Position data class to Exception data class
'*  2015/09/03  Supragyan         Modified Index Action method
'*  2015/09/04  Supragyan         Modified ArrayList to List of ExceptionData on Index action method
'**********************************************************************************

'system
Imports System
Imports System.Web.Mvc
Imports System.Collections.Generic

' フレームワーク
Imports Touryo.Infrastructure.Framework.Util

' 部品
Imports Touryo.Infrastructure.Public.Str

Namespace Controllers
#Region "ErrorController"
    ''' <summary>
    ''' ErrorController class
    ''' </summary>
    Public Class ErrorController
        Inherits Controller
        ''' <summary>Session情報：リピータ処理用</summary>
        Private listData As New List(Of ExceptionData)()

#Region "Index"
        ''' <summary>
        ''' Index Action method to display an error message error information on the screen
        ''' </summary>
        ''' <returns></returns>
        Public Function Index() As ActionResult
            'To get an error message from Session
            Dim err_msg As String = DirectCast(Session(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE), String)

            'To get an error information from Session
            Dim err_info As String = DirectCast(Session(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION), String)

            'To encode error message and display on Error screen
            ViewBag.label1Data = CustomEncode.HtmlEncode(err_msg)

            'To encode error information and display on Error screen
            ViewBag.label2Data = CustomEncode.HtmlEncode(err_info)

            ' 画面にセッション情報を表示する------------------------------------------

            If Session IsNot Nothing Then
                'foreach
                For Each strKey As String In Session
                    If Session(strKey) Is Nothing Then
                        'Add key and value to PositionData
                        listData.Add(New ExceptionData(strKey, "null"))
                    Else
                        'Add key and value to PositionData
                        listData.Add(New ExceptionData(strKey, CustomEncode.HtmlEncode(Session(strKey).ToString())))
                    End If
                Next
                'データバインド
                ViewBag.datas = listData
            End If

            If Session(FxHttpContextIndex.SESSION_ABANDON_FLAG) IsNot Nothing Then
                ' セッション情報を削除する------------------------------------------------
                If CBool(Session(FxHttpContextIndex.SESSION_ABANDON_FLAG)) Then
                    ' セッション タイムアウト検出用Cookieを消去
                    ' ※ Removeが正常に動作しないため、値を空文字に設定 ＝ 消去とする

                    ' Set-Cookie HTTPヘッダをレスポンス
                    Response.Cookies.[Set](FxCmnFunction.DeleteCookieForSessionTimeoutDetection())

                    Try
                        'To store error information from session before clear the session
                        ErrorInformation.ErrorMessage = DirectCast(Session(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE), String)
                        ErrorInformation.ErrorInfo = DirectCast(Session(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION), String)
                        ErrorInformation.ErrorDatas = listData

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
            End If
            Return View()
        End Function

#End Region
    End Class

#End Region

#Region "ExceptionData"

    ''' <summary>
    ''' ExceptionData class to set key and value for throwing exception 
    ''' </summary>
    Public Class ExceptionData
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

